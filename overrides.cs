package ScrabbleOverrides {
	function serverCmdSuicide(%client) {
		%client.play2D(errorSound);
		messageClient(%client, '', "\c6Suicide has been \c0disabled \c6for this gamemode.");
		return;
	}
};
activatePackage(ScrabbleOverrides);