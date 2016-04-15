// list of words you can use: https://github.com/dwyl/english-words

if($Pref::Server::Scrabble::WordList $= "") {
	$Pref::Server::Scrabble::WordList = "config/server/scrabble/words.txt";
}

function isScrabbleWord(%word) {
	return $Scrabble::IsWord[%word] ? 1 : 0;
}

function initScrabbleWords() {
	if(!isFile($Pref::Server::Scrabble::WordList)) {
		echo("WORDLIST DOES NOT EXIST:" SPC $Pref::Server::Scrabble::WordList);
		return;
	}

	%file = new FileObject();
	%file.openForRead($Pref::Server::Scrabble::WordList);

	$Scrabble::WordCount = 0;

	while(!%file.isEOF()) {
		%word = %file.readLine();

		if(strLen(%word) > 2) {
			$Scrabble::IsWord[%word] = 1;
			$Scrabble::WordCount++;
		}
	}

	%file.close();
	%file.delete();
}

if(!$Scrabble::WordCount) {
	initScrabbleWords();
}