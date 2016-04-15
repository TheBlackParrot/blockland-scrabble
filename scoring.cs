// clarification on scoring multiple premium squares: http://boardgames.stackexchange.com/a/9120

function getScrabbleWordScore(%word, %mods) {
	%total = 0;

	%ov_mult = 1;

	for(%i=0;%i<strLen(%word);%i++) {
		%char = strUpr(getSubStr(%word, %i, 1));
		%mod = getField(%mods, %i);

		%ch_mult = 1;

		if(%mod !$= "ST" && %mod !$= "NN") {
			if(strStr(%mod, "W") != -1) {
				%ov_mult = %ov_mult * getSubStr(%mod, 0, 1);
			}

			if(strStr(%mod, "L") != -1) {
				%ch_mult = getSubStr(%mod, 0, 1);
			}
		}

		%score += %ch_mult * $Scrabble::Points[%char];
	}

	return %score * %ov_mult;
}