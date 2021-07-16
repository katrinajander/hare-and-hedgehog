using static System.Console;
using static Board.SqType;

class Board{

    public enum SqType {None, Carrot, Hedgehog, Rabbit, Salad, SQ_156, SQ_2, SQ_3, SQ_4}   

    public Player[] players;
    public bool[] positions; //true if there is a player at that position
    public int curPlayer;
    public int numWinners; //to calculate how many carrots to finish with legally

    static SqType[] squares = {None, Rabbit, Carrot, Rabbit, SQ_3, Carrot, Rabbit, 
                                Salad, Hedgehog, SQ_4, SQ_2, Hedgehog, SQ_3, Carrot, Rabbit, Hedgehog, 
                                SQ_156, SQ_2, SQ_4, Hedgehog, SQ_3, Carrot, Salad, SQ_2, Hedgehog, 
                                Rabbit,  Carrot, SQ_4, SQ_3, SQ_2, Hedgehog, Rabbit, SQ_156, Carrot, 
                                Hedgehog, SQ_2, SQ_3, Hedgehog, Carrot, Rabbit, Carrot, SQ_2, Salad, 
                                Hedgehog, SqType.SQ_3, SQ_4, Rabbit, SQ_2, SQ_156, Carrot, Hedgehog, Rabbit, 
                                SQ_3, SQ_2, SQ_4, Carrot, Hedgehog, Salad, Rabbit, Carrot, SQ_2, 
                                Rabbit, Salad, Rabbit}; //first square is 1

    public Board(Player[] curPlayers){
        players = curPlayers;
        positions = new bool[65]; //63 squares and 64 as the finish (whether a player is at 0 does not matter)
        curPlayer = 0; //the starting player is 0
        numWinners = 1; //to get home first you need <= 10 carrots
    }

    public int cost(int start, int end){ //calculates the cost of a move from start/end positions
        if(end == start){ //staying in the same spot (only on rabbit and carrot spaces)
            return 0;
        }
        else if(end > start){ //moving forwards
            int n = end - start;
            return n * (n + 1) / 2; //lose triangle number of spaces carrots
        }
        else{ //moving backwards
            int n = start - end;
            return 10 * n; //gain 10 times # of spaces carrots
        }
    }

    //returns true if a space is occupied by a player
    public bool isOccupied(int pos){
        return positions[pos];
    }

    //returns the order of the players on the board
    //order[0] is the player in first place
    public Player[] playerOrder(){
        Player[] order = new Player[players.Length];
        int index = 0;
        for(int j = 64; j >= 0; j--){
            if(isOccupied(j)){
                foreach(Player p in players){
                    if(p.pos == j){
                        order[index] = p;
                    }
                }
                index++;
            }
        }
        return order;
    }

    public void updateCurPlayer(){
        int nextPlayer = curPlayer + 1;
        if(nextPlayer >= players.Length){
            nextPlayer = 0;
        }
        //see if the next player's turn needs to be skipped (due to eating salad or winning)
        while(true){
            if(!players[nextPlayer].eatingSalad){
                break;
            }
            else{ //skip player's turn
                players[nextPlayer].eatingSalad = false;
                nextPlayer++;
                if(nextPlayer >= players.Length){
                    nextPlayer = 0;
                }
            }
            if(!players[nextPlayer].won){
                break;
            }
            else{ //skip player's turn
                nextPlayer++;
                if(nextPlayer >= players.Length){
                    nextPlayer = 0;
                }
            }
        }
        curPlayer = nextPlayer;
        rabbitSquare(players[curPlayer]);
        numberSquare(players[curPlayer]);
    }

    //checks if the player currently moving passes a rabbit square with another player
    //and increments numPassed for that player
    //direction is moving forwards (true) or backwards (false)
    public void checkPassing(int startPos, int endPos, bool dir){
        if(dir){
            for(int i = startPos; i < endPos; i++){
                //if passing a rabbit square
                if(squares[i] == Rabbit){
                    foreach(Player otherPlayer in players){
                        if(otherPlayer.pos == i){
                            otherPlayer.numPassed++;
                        }
                    }
                }
            }
        }
        else{
            for(int i = startPos; i > endPos; i--){
                //if passing a rabbit square
                if(squares[i] == Rabbit){
                    foreach(Player otherPlayer in players){
                        if(otherPlayer.pos == i){
                            otherPlayer.numPassed++;
                        }
                    }
                }
            }
        }
    }

    //awarding/taking carrots for rabbit squares
    //called whenever the current player changes (start of a turn)
    public void rabbitSquare(Player p){
        SqType curSquare = squares[p.pos];
        if(curSquare == Rabbit){
            if(p.numPassed == 0){ //no players passed the current player
                p.carrots += 10; //get 10 carrots
            }
            else{
                p.carrots -= 10 * p.numPassed;
            }
        }
        p.numPassed = 0;
    }

    //awarding carrots for number squares
    //called whenever current player changes (start of a turn)
    public void numberSquare(Player p){
        //awarding carrots if on a number square
        Player[] order = playerOrder();
        /*WriteLine("the order at the start of " + p.color + "'s turn is:");
        foreach(Player player in order){
            if(player != null)
                WriteLine(player.color);
        }
        WriteLine();*/
        int place = 0; //which place the current player is in
        for(int i = 0; i < order.Length; i++){
            if(p == order[i]){
                place = i;
            }
        }
        place++;
        
        SqType curSquare = squares[p.pos];
        if(curSquare == SQ_156 || curSquare == SQ_2 || curSquare == SQ_3 || curSquare == SQ_4){
            //WriteLine(p.color + " is on a number square and in " + place + " place");
            if(curSquare == SQ_156){
                if(place == 1 || place == 5 || place == 6){
                    p.carrots += place * 10;
                }
            }
            else{ //square is either 2, 3, or 4
                if(int.Parse(curSquare.ToString()[curSquare.ToString().Length - 1].ToString()) == place){
                    p.carrots += place * 10;
                }
            }
        }
    }

    //makes the current player move to position pos and returns true if legal
    public bool move(int newPos) {
        Player p = players[curPlayer];
        int c = cost(p.pos, newPos);

        if(newPos == p.pos){ //staying in the same spot (only on rabbit and carrot spaces)
            if(squares[newPos] == Rabbit || squares[newPos] == Carrot){
                if(squares[newPos] == Carrot){ 
                    p.carrots += 10;
                    //WriteLine("awarded " + p.color +" carrots for staying on carrot square");
                }
                updateCurPlayer();
                return true;
            }
        }

        //moving forwards
        else if(newPos > p.pos){ //moving forwards
            checkPassing(p.pos, newPos, true);
            //if the player is crossing the finish line (space can be occupied)
            if(newPos == 64){ 
                if(p.carrots - c <= (numWinners * 10) && p.salads == 0){
                    p.won = true;
                }
            }
            //if the player can pay for the move, is not moving forward onto a hedgehog, and an empty space
            if(c < p.carrots && squares[newPos] != Hedgehog && !isOccupied(newPos)){
                if(squares[newPos] == Salad){
                    p.eatingSalad = true;
                    p.salads--; //eat a salad
                }
                p.carrots -= c; //spend the carrots
                positions[p.pos] = false;
                positions[newPos] = true; // moved to new spot
                p.pos = newPos;
                updateCurPlayer();
                return true;
            }
        }
        else{ //moving backwards
            checkPassing(p.pos, newPos, false);
            if(squares[newPos] == Hedgehog && !isOccupied(newPos)){
                p.carrots += c; //gain carrots
                positions[p.pos] = false;
                positions[newPos] = true; //moved to new spot
                p.pos = newPos;
                updateCurPlayer();
                return true;
            }
        }
        return false;
    }
}