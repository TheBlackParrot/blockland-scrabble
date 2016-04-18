// todo

// idea: let player select which pieces will be in play
// if no piece is present, assume the player is putting down the selected piece in the bottom print
// if a piece is present, assume the player is using the tile clicked on
// color tiles yellow regardless, new tiles will be ghost bricks until selection is confirmed a valid play
// reverse the selection as well to double check, if the reverse equals the same thing, disregard it

// servercommands to package:
//   - shiftBrick (1 0 0 = up, -1 0 0 = down, 0 1 0 = left, 0 -1 0 = right)
//     - left/right will change the selected piece, wrap it around at the ends

// bottomprint example
function GameConnection::exbp(%this) {
	%str = "\c6[R1] \c6[T1] \c0[E1] \c6[E1] \c6[S1] \c6[B3] \c6[X8]";

	%this.bottomPrint(%str, 9, 1);
}

function GameConnection::doBottomPrint(%this) {
	cancel(%this.bpsched);
	%this.bpsched = %this.schedule(1000, doBottomPrint);

	%str = "";

	for(%i=0;%i<getWordCount(%this.pieces);%i++) {
		%piece = getWord(%this.pieces, %i);

		if(%i == %this.activePiece) {
			%str = trim(%str SPC "\c0[" @ %piece @ "]");
		} else {
			%str = trim(%str SPC "\c6[" @ %piece @ "]");
		}
	}

	%this.bottomPrint(%str, 3, 1);
}

function GameConnection::switchActivePiece(%this, %dir) {
	%active = %this.activePiece ? %this.activePiece : 0;

	switch$(%dir) {
		case "left": %active--;
		case "right": %active++;
	}

	if(%active < 0) {
		%active = getWordCount(%this.pieces)-1;
	}
	if(%active >= getWordCount(%this.pieces)) {
		%active = 0;
	}

	%this.activePiece = %active;

	%this.play2D(brickRotateSound);
	%this.doBottomPrint();
}

package ScrabbleInteractionPackage {
	function serverCmdSuperShiftBrick(%client, %t1, %t2, %t3) {
		return serverCmdShiftBrick(%client, %t1, %t2, %t3);
	}

	function serverCmdShiftBrick(%client, %t1, %t2, %t3) {
		%vec = %t1 SPC %t2 SPC %t3;

		switch$(%vec) {
			case "0 1 0": %client.switchActivePiece("left");
			case "0 -1 0": %client.switchActivePiece("right");
		}
	}

	function serverCmdUndoBrick(%client) {
		if(%client.selectionSet.getCount() > 0) {
			%row = %client.selectionSet.getObject(%client.selectionSet.getCount()-1);
			%client.selectionSet.removeBrick(%row.brick);
		}
	}

	function serverCmdCancelBrick(%client) {
		if(%client.selectionSet.getCount() > 0) {
			while(%client.selectionSet.getCount() > 0) {
				%row = %client.selectionSet.getObject(0);
				%client.selectionSet.removeBrick(%row.brick);
			}
		}
	}

	function serverCmdPlantBrick(%client) {
		if(%client.selectionSet.getCount() > 0) {
			%client.selectionSet.plant();
		}
	}
};
activatePackage(ScrabbleInteractionPackage);