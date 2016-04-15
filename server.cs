ForceRequiredAddOn("Print_2x2f_Scrabble");

if(!$Scrabble::Init) {
	$Scrabble::Init = 1;
	exec("./bricks/main.cs");
}

exec("./board.cs");
exec("./pieces.cs");
exec("./scoring.cs");
exec("./words.cs");
exec("./interaction.cs");
exec("./overrides.cs");
exec("./system.cs");
exec("./commands.cs");

package mainScrabblePackage {
	function onServerDestroyed() {
		deleteVariables("$Scrabble::*");
		parent::onServerDestroyed();
	}
};
activatePackage(mainScrabblePackage);