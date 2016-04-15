// turns should last 2 minutes up until 8 words are placed, when it will go up to 3 minutes
// game ends on a player quitting, winner determined automatically
//		may need to separate forfeiting/ending, ending should be agreed upon both players, forfeiting should be an automatic loss for the forfeiter

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
		%brick.delete();
		%row.delete();
		return;
	}
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

		for(%i=0;%i<7;%i++) {
			%client.pieces = trim(%client.pieces SPC %this.drawPiece());
		}

		%client.selectionSet = new ScriptGroup(SelectionSet);
	}

	createBoard();
}