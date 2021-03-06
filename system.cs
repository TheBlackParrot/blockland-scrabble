// turns should last 2 minutes up until 8 words are placed, when it will go up to 3 minutes
// game ends on a player quitting, winner determined automatically
//		may need to separate forfeiting/ending, ending should be agreed upon both players, forfeiting should be an automatic loss for the forfeiter


// NOTES
// when selections are confirmed a word, create a new set that refers to those tiles, set a variable in each tile pointing to the set
// for each tile, check every surrounding tile for an existing word and add the letters on to see if it's a valid word.
// if the direction goes an opposite way, deem it invalid
// word sets should contain: all tiles, start x, start y, end x, end y

function SelectionSet::contains(%this, %brick) {
	for(%i=0;%i<%this.getCount();%i++) {
		%cur = %this.getObject(%i);
		if(%cur.brick == %brick) {
			return %cur;
		}
	}

	return 0;
}

function SelectionSet::addBrick(%this, %brick) {
	if(%this.contains(%brick)) {
		return;
	}

	if(!isObject(%brick)) {
		return;
	}

	if(!%brick.isPlanted) {
		%this.client.removePiece();
	}

	%brick.setColor(6);
	%brick.setColorFX(3);

	%new = new ScriptObject(PieceObj) {
		brick = %brick;
	};

	serverPlay3D(brickPlantSound, %brick.getPosition());
	%this.add(%new);
}

function SelectionSet::removeBrick(%this, %brick) {
	%row = %this.contains(%brick);

	if(!%row) {
		return;
	}

	// double checking, not using %brick here on purpose
	if(!isObject(%row.brick)) {
		return;
	}

	serverPlay3D(brickMoveSound, %brick.getPosition());

	if(%brick.isPlanted) {
		// not deleting yet
		%brick.setColor(5);
		%brick.setColorFX(0);
		%row.delete();
		return;
	} else {
		%client = %this.client;
		%client.addPiece(%brick.letter);

		%brick.delete();
		%row.delete();
		return;
	}
}

function SelectionSet::determineDirection(%this) {
	if(%this.getCount() < 2) {
		return "invalid";
	}

	for(%i=0;%i<%this.getCount();%i++) {
		%row = %this.getObject(%i);
		%brick = %row.brick;

		if(%i > 0) {
			if(%prev[x] == %brick.x) {
				return "down";
			}
			if(%prev[y] == %brick.y) {
				return "right";
			}
		}

		%prev[x] = %brick.x;
		%prev[y] = %brick.y;
	}

	return "invalid";
}

function SelectionSet::isWord(%this) {
	// in scrabble, words can only go down/right, so we need to make sure the set is in order
	%dir = %this.determineDirection();

	if(%dir $= "invalid") {
		return;
	}

	%lowest[pos] = 16;
	%highest[pos] = -1;
	%lowest[row] = 0;
	%highest[row] = 0;

	for(%i=0;%i<%this.getCount();%i++) {
		%row = %this.getObject(%i);
		%brick = %row.brick;

		switch$(%dir) {
			case "down":
				if(%brick.y > %highest[pos]) {
					%highest[pos] = %brick.y;
					%highest[row] = %row;
				}

				if(%brick.y < %lowest[pos]) {
					%lowest[pos] = %brick.y;
					%lowest[row] = %row;
				}

				%ordered[%brick.y] = %row;

			case "right":
				if(%brick.x > %highest[pos]) {
					%highest[pos] = %brick.x;
					%highest[row] = %row;
				}

				if(%brick.x < %lowest[pos]) {
					%lowest[pos] = %brick.x;
					%lowest[row] = %row;
				}

				%ordered[%brick.x] = %row;
		}
	}

	switch$(%dir) {
		case "right":
			for(%i=%lowest[pos];%i<=%highest[pos];%i++) {
				%row = %ordered[%i];
				%brick = %row.brick;

				%str = %str @ %brick.letter;
				%tiles = trim(%tiles TAB %brick.tile);
				%rows = trim(%rows TAB %row);
			}

		case "down":
			for(%i=%highest[pos];%i>=%lowest[pos];%i--) {
				%row = %ordered[%i];
				%brick = %row.brick;

				%str = %str @ %brick.letter;
				%tiles = trim(%tiles TAB %brick.tile);
				%rows = trim(%rows TAB %row);
			}
	}

	return isScrabbleWord(%str) NL %str NL %tiles NL %rows NL (%lowest[pos] TAB %ordered[%lowest[pos]]) NL (%highest[pos] TAB %ordered[%highest[pos]]);
}

function SelectionSet::setEntireChain(%this) {
	%dir = %this.determineDirection();
	
	%word_data = %this.isWord();
	%rows = getRecord(%word_data, 3);

	talk(%word_data);
	talk(%rows);

	for(%i=0;%i<getFieldCount(%rows);%i++) {
		%row = getField(%rows, %i);
		%brick = %row.brick;

		talk("CHECKING CHAIN:" SPC %row SPC %row.brick);

		%surrounds = %brick.getSurroundingPieces();
		talk(%surrounds);

		switch$(%dir) {
			case "right":
				talk("CHECKING HORIZONTALLY");
				%left_s = getField(%surrounds, 0);
				%right_s = getField(%surrounds, 1);

				while(isObject(%left_s)) {
					%this.addBrick(%left_s);

					%left_s = getField(%left_s.getSurroundingPieces(), 0);
				}

				while(isObject(%right_s)) {
					%this.addBrick(%right_s);

					%right_s = getField(%right_s.getSurroundingPieces(), 1);
				}

			case "down":
				talk("CHECKING VERTICALLY");
				%up_s = getField(%surrounds, 2);
				%down_s = getField(%surrounds, 3);

				while(isObject(%up_s)) {
					%this.addBrick(%up_s);

					%up_s = getField(%up_s.getSurroundingPieces(), 2);
				}

				while(isObject(%down_s)) {
					%this.addBrick(%down_s);

					%down_s = getField(%down_s.getSurroundingPieces(), 3);
				}
		}
	}
}

function SelectionSet::plant(%this) {
	%this.setEntireChain();

	%word_data = %this.isWord();
	%isWord = getRecord(%word_data, 0);
	%word = getRecord(%word_data, 1);
	%tiles = getRecord(%word_data, 2);
	%rows = getRecord(%word_data, 3);

	talk(%word);

	if(!%isWord) {
		return;
	}

	%add = 0;
	for(%i=0;%i<%this.getCount();%i++) {
		%row = %this.getObject(%i);
		%brick = %row.brick;

		if(!%brick.isPlanted) {
			%add++;
		}
	}

	if(!%add) {
		return;
	}

	for(%i=0;%i<%this.getCount();%i++) {
		%row = %this.getObject(%i);
		%brick = %row.brick;

		if(!%brick.isPlanted) {
			%brick.plant();
		}

		// clever
		if(%i < %add) {
			%this.client.drawPiece();
		}

		%brick.setColor(5);
		%brick.setColorFX(0);
	}

	while(%this.getCount() > 0) {
		%this.getObject(0).delete();
	}

	%score = getScrabbleWordScore(%word, %tiles);
	talk(%score SPC "POINTS, " SPC %word SPC "::" SPC %tiles);

	%this.client.score += %score;
}

function ScrabbleGame::init(%this, %players) {
	%this.pieces = $Scrabble::Pieces;
	%this.gameNum = $Scrabble::Games;
	%this.playerCount = 0;
	%this.amountInPlay = 0;

	for(%i=0;%i<getFieldCount(%players);%i++) {
		%client = getField(%players, %i);

		if(!isObject(%client)) {
			continue;
		}

		if(isObject(%client.player)) {
			%client.player.setTransform($Scrabble::Offset);
		}

		%this.players[%i] = %client;
		%this.playerCount++;

		%client.scrabbleGame = %this;

		%client.pieces = "";
		for(%i=0;%i<7;%i++) {
			%client.pieces = trim(%client.pieces SPC %this.drawPiece());
		}

		%client.selectionSet = new ScriptGroup(SelectionSet) {
			client = %client;
		};
	}

	createBoard();
}