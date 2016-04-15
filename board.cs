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
		case "3W": return 17;
		case "2W": return 21;
		case "3L": return 18;
		case "2L": return 22;
		case "ST": return 20;
		case "NN": return 19;
	}
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
			};
			%brick.setName(%name);

			%brick.plant();
			%brick.setTrusted(1);

			BrickGroup_888888.add(%brick);
		}
	}

	$Scrabble::Offset = vectorAdd($Scrabble::Offset, "100 0 0");
}