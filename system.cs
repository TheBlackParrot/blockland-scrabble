// turns should last 2 minutes up until 8 words are placed, when it will go up to 3 minutes
// game ends on a player quitting, winner determined automatically
//		may need to separate forfeiting/ending, ending should be agreed upon both players, forfeiting should be an automatic loss for the forfeiter

function ScrabbleGame::init(%this, %players, %gameNum) {
	%this.pieces = $Scrabble::Pieces;
	%this.gameNum = %gameNum;
	%this.playerCount = 0;

	for(%i=0;%i<getFieldCount(%players);%i++) {
		%client = getField(%players, %i);

		if(!isObject(%client)) {
			continue;
		}

		%this.players[%i] = %client;
		%this.playerCount++;

		%client.scrabbleGame = %this;

		for(%i=0;%i<7;%i++) {
			%client.pieces = trim(%client.pieces SPC %this.drawPiece());
		}
	}
}