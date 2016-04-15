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