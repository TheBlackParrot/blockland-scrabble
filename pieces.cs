function initScrabblePoints() {
	%points[1] = "EAIONRTLSU";
	%points[2] = "DG";
	%points[3] = "BCMP";
	%points[4] = "FHVWY";
	%points[5] = "K";
	%points[8] = "JX";
	%points[10] = "QZ";

	%amnts = "1 2 3 4 5 8 10";

	for(%id=0;%id<getWordCount(%amnts);%id++) {
		%amnt = getWord(%amnts, %id);

		for(%pid=0;%pid<strLen(%points[%amnt]);%pid++) {
			%char = getSubStr(%points[%amnt], %pid, 1);

			$Scrabble::Points[%char] = %amnt;
		}
	}
}

function initScrabblePieces() {
	for(%i=0;%i<12;%i++) { %pieces = trim(%pieces SPC "E"); }

	for(%i=0;%i<9;%i++) { 
		%pieces = trim(%pieces SPC "A");
		%pieces = trim(%pieces SPC "I");
	}

	for(%i=0;%i<8;%i++) { %pieces = trim(%pieces SPC "O"); }

	for(%i=0;%i<6;%i++) { 
		%pieces = trim(%pieces SPC "N");
		%pieces = trim(%pieces SPC "R");
		%pieces = trim(%pieces SPC "T");
	}

	for(%i=0;%i<4;%i++) { 
		%pieces = trim(%pieces SPC "L");
		%pieces = trim(%pieces SPC "S");
		%pieces = trim(%pieces SPC "U");
		%pieces = trim(%pieces SPC "D");
	}

	for(%i=0;%i<3;%i++) { %pieces = trim(%pieces SPC "G"); }

	for(%i=0;%i<2;%i++) { 
		%pieces = trim(%pieces SPC "B");
		%pieces = trim(%pieces SPC "C");
		%pieces = trim(%pieces SPC "M");
		%pieces = trim(%pieces SPC "P");
		%pieces = trim(%pieces SPC "F");
		%pieces = trim(%pieces SPC "H");
		%pieces = trim(%pieces SPC "V");
		%pieces = trim(%pieces SPC "W");
		%pieces = trim(%pieces SPC "Y");
	}

	$Scrabble::Pieces = trim(%pieces SPC "K J X Q Z");
}
if($Scrabble::Pieces $= "") {
	initScrabblePieces();
	initScrabblePoints();
}

function getScrabblePrint(%char) {
	switch$(%char) {
		case "A": return 48;
		case "B": return 47;
		case "C": return 40;
		case "D": return 39;
		case "E": return 38;
		case "F": return 37;
		case "G": return 36;
		case "H": return 35;
		case "I": return 34;
		case "J": return 33;
		case "K": return 32;
		case "L": return 31;
		case "M": return 30;
		case "N": return 29;
		case "O": return 28;
		case "P": return 27;
		case "Q": return 26;
		case "R": return 25;
		case "S": return 24;
		case "T": return 23;
		case "U": return 22;
		case "V": return 21;
		case "W": return 20;
		case "X": return 19;
		case "Y": return 18;
		case "Z": return 17;
	}
}

function ScrabbleGame::drawPiece(%this) {
	if(getWordCount(%this.pieces) < 1) {
		return -1;
	}
	
	%pieceID = getRandom(0, getWordCount(%this.pieces)-1);
	%piece = getWord(%this.pieces, %pieceID);

	for(%i=0;%i<getWordCount(%this.pieces);%i++) {
		if(%i != %pieceID) {
			%new_pieces = trim(%new_pieces SPC getWord(%this.pieces, %i));
		}
	}

	%this.pieces = %new_pieces;

	return %piece;
}