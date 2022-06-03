namespace SweetFields.Generate;

class Matrix
{
    Random random = null;
    public int AllWordsCount { get; set; }

    public Matrix(List<Generate.Word> _words, int _level, int _rows, int _columns)
    {
        this.Words = _words;
        this.Level = _level;
        this.Rows = _rows;
        this.Columns = _columns;

        random = new Random();
        AllWordsCount = 0;
    }

    public Tuple<char, Generate.Word>[,] Answer { get; set; }
    public List<Generate.Word> Words { get; set; }
    public int Level { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }

    public bool Generate()
    {
        Tuple<char, Generate.Word>[,] tempMatrixWords = AddWords(new Tuple<char, Generate.Word>[this.Columns, this.Rows]);

        this.Answer = (tempMatrixWords != null) ? tempMatrixWords : new Tuple<char, Generate.Word>[this.Columns, this.Rows];

        return (tempMatrixWords != null) ? true : false;
    }

    private Tuple<char, Generate.Word>[,] AddWords(Tuple<char, Generate.Word>[,] tempMatrix)
    {
        tempMatrix = AddGeneralWord(tempMatrix);

        if (tempMatrix != null)
        {
            for (int i = 0; i < this.Words.Where(x => x.IsUsed).ToList<Word>().First().Answer.ToArray().Length - 1; i++)
                tempMatrix = AddRegularWord(tempMatrix, i);

            tempMatrix = FillGeneralWord(tempMatrix);
        }

        return tempMatrix;
    }

    List<Word> GetGeneralWords()
    {
        List<Word> answer = new List<Word>();

        foreach (Word tempWord in this.Words.Where(x => (x.Level <= this.Level && x.Answer.Length <= Math.Min(this.Rows, this.Columns) && !x.IsUsed)).ToList<Word>())
        {
            int AllWordsCount = 0;

            foreach (char tempWordSymbol in tempWord.Answer)
            {
                bool isHasSymbol = false;

                foreach (Word tempWordForCompare in this.Words.Where(x => (x.Level <= this.Level && x.Answer.Length <= Math.Min(this.Rows, this.Columns) && !x.IsUsed && x.ID != tempWord.ID)).ToList<Word>())
                {
                    int indexOfSymbolTempWord = Array.FindIndex(tempWord.Answer.ToArray(), x => x == tempWordSymbol);
                    int indexOfSymboltempWordForCompare = Array.FindIndex(tempWordForCompare.Answer.ToArray(), x => x == tempWordSymbol);

                    tempWord.Location = new Point(Convert.ToInt16(Math.Round((double)this.Columns / 2)), indexOfSymbolTempWord);

                    if (indexOfSymbolTempWord != -1
                        && indexOfSymboltempWordForCompare != -1
                        && ((((int)tempWord.Location.X) + (tempWord.Answer.Length - indexOfSymbolTempWord)) <= this.Columns
                        && (((int)tempWord.Location.X) - indexOfSymbolTempWord) >= 0))
                        isHasSymbol = true;
                }

                if (isHasSymbol)
                    AllWordsCount++;
            }

            if (AllWordsCount >= 3)
                answer.Add(tempWord);
        }

        return answer;
    }

    private Tuple<char, Generate.Word>[,] AddGeneralWord(Tuple<char, Generate.Word>[,] answer)
    {
        List<Word> generalWords = GetGeneralWords();

        if (generalWords.Count() > 0)
        {
            AllWordsCount = 0;

            Word tempWord = generalWords[random.Next(0, generalWords.Count())];

            AllWordsCount++;

            tempWord.Number = AllWordsCount;
            tempWord.IsUsed = true;
            tempWord.IsVertical = true;
            tempWord.Location = new Point(Convert.ToInt16(Math.Round((double)this.Columns / 2)) - 1, random.Next((this.Rows - tempWord.Answer.Length) > 0 ? 1 : 0, this.Rows - tempWord.Answer.Length));

            for (int i = 0; i < tempWord.Answer.Length; i++)
                answer[((int)tempWord.Location.X), ((int)tempWord.Location.Y) + i] = Tuple.Create(tempWord.Answer[i], tempWord);

            this.Words[tempWord.ID] = tempWord;

            return answer;
        }
        else return null;
    }

    private Tuple<char, Generate.Word>[,] FillGeneralWord(Tuple<char, Generate.Word>[,] answer)
    {
        Word tempWord = this.Words.Where(x => x.IsVertical && x.IsUsed).ToList<Word>().First();

        for (int i = 0; i < tempWord.Answer.Length; i++)
            answer[((int)tempWord.Location.X), ((int)tempWord.Location.Y) + i] = Tuple.Create(tempWord.Answer[i], tempWord);

        return answer;
    }

    private Tuple<char, Generate.Word>[,] AddRegularWord(Tuple<char, Generate.Word>[,] temp_matrix, int column)
    {
        Word wordUsedGeneral = this.Words.Where(x => x.IsVertical && x.IsUsed).ToList<Word>().First();
        char tempWordUsedSymbol = wordUsedGeneral.Answer[column];
        System.Console.WriteLine(tempWordUsedSymbol);

        foreach (Word tempNonUsedWord in this.Words.Where(x => (x.Level <= this.Level && x.Answer.Length <= Math.Min(this.Columns, this.Rows) && !x.IsUsed)).ToList<Word>())
        {
            int indexOfSymbolTempNonUsedWord = Array.FindIndex(tempNonUsedWord.Answer.ToArray(), x => x == tempWordUsedSymbol);
            int indexOfSymbolUsedGeneral = Array.FindIndex(wordUsedGeneral.Answer.ToArray(), x => x == tempWordUsedSymbol);

            tempNonUsedWord.Location = new Point(wordUsedGeneral.Location.X, wordUsedGeneral.Location.Y + column);

            if (indexOfSymbolTempNonUsedWord != -1
                && indexOfSymbolUsedGeneral != -1
                && (((int)tempNonUsedWord.Location.X) + (tempNonUsedWord.Answer.Length - indexOfSymbolTempNonUsedWord)) <= this.Columns
                && (((int)tempNonUsedWord.Location.X) - (tempNonUsedWord.Answer.Length - (tempNonUsedWord.Answer.Length - indexOfSymbolTempNonUsedWord))) >= 0)
            {
                for (int i = 0; i < (tempNonUsedWord.Answer.Length - (tempNonUsedWord.Answer.Length - indexOfSymbolTempNonUsedWord)); i++)
                    temp_matrix[((int)tempNonUsedWord.Location.X) - (indexOfSymbolTempNonUsedWord - i), ((int)tempNonUsedWord.Location.Y)] = Tuple.Create(tempNonUsedWord.Answer[i], tempNonUsedWord);

                for (int i = -1; i < (tempNonUsedWord.Answer.Length - indexOfSymbolTempNonUsedWord); i++)
                    temp_matrix[((int)tempNonUsedWord.Location.X) + ((i < 0) ? 0 : i), ((int)tempNonUsedWord.Location.Y)] = Tuple.Create(tempNonUsedWord.Answer[(indexOfSymbolTempNonUsedWord + ((i < 0) ? 0 : i))], tempNonUsedWord);

                AllWordsCount++;

                tempNonUsedWord.Number = AllWordsCount;
                tempNonUsedWord.IsUsed = true;
                tempNonUsedWord.IsHorizontal = true;

                this.Words[tempNonUsedWord.ID] = tempNonUsedWord;

                return temp_matrix;
            }
        }

        return temp_matrix;
    }

    public string View()
    {
        string temp_result = "";

        for (int j = 0; j < this.Rows; j++)
        {
            for (int i = 0; i < this.Columns; i++)
                if (this.Answer[i, j]?.Item1.ToString() != null)
                    temp_result += this.Answer[i, j].Item1.ToString() + " ";
                else temp_result += "* ";

            temp_result += "\n";
        }

        return temp_result;
    }
}
