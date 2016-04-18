if($Scrabble::Offset $= "") {
	$Scrabble::Offset = "0 0 0";
}

if($Scrabble::Games $= "") {
	$Scrabble::Games = 0;
}

if($Scrabble::Order[0] $= "") {
	$Scrabble::Order[0]		=	"3W NN NN 2L NN NN NN 3W NN NN NN 2L NN NN 3W";
	$Scrabble::Order[1]		=	"NN 2W NN NN NN 3L NN NN NN 3L NN NN NN 2W NN";
	$Scrabble::Order[2]		=	"NN NN 2W NN NN NN 2L NN 2L NN NN NN 2W NN NN";
	$Scrabble::Order[3]		=	"2L NN NN 2W NN NN NN 2L NN NN NN 2W NN NN 2L";
	$Scrabble::Order[4]		=	"NN NN NN NN 2W NN NN NN NN NN 2W NN NN NN NN";
	$Scrabble::Order[5]		=	"NN 3L NN NN NN 3L NN NN NN 3L NN NN NN 3L NN";
	$Scrabble::Order[6]		=	"NN NN 2L NN NN NN 2L NN 2L NN NN NN 2L NN NN";
	$Scrabble::Order[7]		=	"3W NN NN 2L NN NN NN ST NN NN NN 2L NN NN 3W";
	$Scrabble::Order[8]		=	"NN NN 2L NN NN NN 2L NN 2L NN NN NN 2L NN NN";
	$Scrabble::Order[9]		=	"NN 3L NN NN NN 3L NN NN NN 3L NN NN NN 3L NN";
	$Scrabble::Order[10]	=	"NN NN NN NN 2W NN NN NN NN NN 2W NN NN NN NN";
	$Scrabble::Order[11]	=	"2L NN NN 2W NN NN NN 2L NN NN NN 2W NN NN 2L";
	$Scrabble::Order[12]	=	"NN NN 2W NN NN NN 2L NN 2L NN NN NN 2W NN NN";
	$Scrabble::Order[13]	=	"NN 2W NN NN NN 3L NN NN NN 3L NN NN NN 2W NN";
	$Scrabble::Order[14]	=	"3W NN NN 2L NN NN NN 3W NN NN NN 2L NN NN 3W";
}

function getScrabbleTile(%id) {
	switch$(%id) {
		case "3W": return 41;
		case "2W": return 45;
		case "3L": return 42;
		case "2L": return 46;
		case "ST": return 44;
		case "NN": return 43;
	}
}

function fxDTSBrick::getSurroundingPieces(%this) {
	%left_b = ("ScrabbleBoard_G" @ %this.game @ "_X" @ %this.x-1 @ "_Y" @ %this.y);
	%left = isObject(%left_b.ghostTile) ? %left_b.ghostTile : "";

	%right_b = ("ScrabbleBoard_G" @ %this.game @ "_X" @ %this.x+1 @ "_Y" @ %this.y);
	%right = isObject(%right_b.ghostTile) ? %right_b.ghostTile : "";

	%up_b = ("ScrabbleBoard_G" @ %this.game @ "_X" @ %this.x @ "_Y" @ %this.y-1);
	%up = isObject(%up_b.ghostTile) ? %up_b.ghostTile : "";

	%down_b = ("ScrabbleBoard_G" @ %this.game @ "_X" @ %this.x @ "_Y" @ %this.y+1);
	%down = isObject(%down_b.ghostTile) ? %down_b.ghostTile : "";

	return %left TAB %right TAB %up TAB %down;
}

function createBoard() {
	%dataBlock = "brick3x3FPrintPlateData";
	%boardNum = $Scrabble::Games;

	%sizeX = %dataBlock.brickSizeX/2;
	%sizeY = %dataBlock.brickSizeY/2;

	$Scrabble::Games++;

	for(%x=0;%x<15;%x++) {
		for(%y=0;%y<15;%y++) {
			%name = "ScrabbleBoard_G" @ %boardNum @ "_X" @ %x @ "_Y" @ %y;
			%brick = new fxDTSBrick() {
				angleID = 0;
				client = -1;
				colorFxID = 0;
				colorID = 4;
				dataBlock = %dataBlock;
				isBasePlate = 0;
				isPlanted = 1;
				position = vectorAdd($Scrabble::Offset, %x*%sizeX SPC %y*%sizeY SPC 0.1);
				printID = getScrabbleTile(getWord($Scrabble::Order[%x], %y));
				scale = "1 1 1";
				shapeFxID = 0;
				stackBL_ID = 888888;
				game = %boardNum;
				x = %x;
				y = %y;
				tile = getWord($Scrabble::Order[%x], %y);
				letterPiece = 0;
				boardPiece = 1;
				ghostTile = -1;
			};
			%brick.setName(%name);

			%brick.plant();
			%brick.setTrusted(1);

			BrickGroup_888888.add(%brick);
		}
	}

	$Scrabble::Offset = vectorAdd($Scrabble::Offset, "70 0 0");
}

package ScrabbleBoardPackage {
	function fxDTSBrick::onActivate(%this, %player, %client, %pos, %vec) {
		talk(%this);

		if(%client.scrabbleGame.gameNum != %this.game) {
			talk("NOT THE SAME GAME");
			return parent::onActivate(%this, %player, %client, %pos, %vec);
		}

		if(%this.boardPiece) {
			talk("BOARD PIECE");
			if(!isObject(%this.ghostTile)) {
				talk("NEW GHOST");
				// avoiding giving ownership to the player, hence "owner" being here
				%brick = new fxDTSBrick() {
					angleID = 0;
					client = -1;
					colorFxID = 3;
					colorID = 6;
					dataBlock = %this.getDatablock();
					isBasePlate = 0;
					isPlanted = 0;
					position = vectorAdd(%this.getPosition(), "0 0 0.2");
					printID = getScrabblePrint(getWord(%client.pieces, %client.activePiece));
					scale = "1 1 1";
					shapeFxID = 0;
					stackBL_ID = 888888;
					game = %this.game;
					x = %this.x;
					y = %this.y;
					tile = %this.tile;
					letter = getWord(%client.pieces, %client.activePiece);
					letterPiece = 1;
					boardPiece = 0;
					owner = %client;
					parentTile = %this;
				};

				%brick.setTrusted(1);
				BrickGroup_888888.add(%brick);

				%client.selectionSet.addBrick(%brick);
				%this.ghostTile = %brick;
			} else {
				talk("OLD GHOST");
				%client.selectionSet.removeBrick(%this.ghostTile);
			}

			%client.doBottomPrint();
		}

		if(%this.letterPiece) {
			talk("LETTER PIECE");
			if(%this.isPlanted) {
				talk("PLANTED");
				if(%client.selectionSet.contains(%this)) {
					talk("SELECTED");
					%client.selectionSet.removeBrick(%this);
				} else {
					talk("NOT SELECTED");
					%client.selectionSet.addBrick(%this);
				}
			}
		}
	}
};
activatePackage(ScrabbleBoardPackage);