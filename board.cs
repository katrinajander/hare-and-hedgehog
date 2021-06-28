using static System.Console;
class Board{

    public static Player[] players;
    public static bool[] positions; //true if there is a player at that position
    public static int curPlayer;
    public int numWinners; //to calculate how many carrots to finish with legally

    enum SqType {CARROT, HEDGEHOG, RABBIT, SALAD, SQ_156, SQ_2, SQ_3, SQ_4}

    static string[] squares = {"", SqType.CARROT.ToString(),  SqType.CARROT.ToString(),  SqType.RABBIT.ToString(), 
                                SqType.SQ_3.ToString(),  SqType.CARROT.ToString(), SqType.RABBIT.ToString(), 
                                SqType.SALAD.ToString(), SqType.HEDGEHOG.ToString(), SqType.SQ_4.ToString(), 
                                SqType.SQ_2.ToString(), SqType.HEDGEHOG.ToString(), SqType.SQ_3.ToString(),  
                                SqType.CARROT.ToString(), SqType.RABBIT.ToString(), SqType.HEDGEHOG.ToString(), 
                                SqType.SQ_156.ToString(), SqType.SQ_2.ToString(), SqType.SQ_4.ToString(), 
                                SqType.HEDGEHOG.ToString(), SqType.SQ_3.ToString(), SqType.CARROT.ToString(), 
                                SqType.SALAD.ToString(), SqType.SQ_2.ToString(), SqType.HEDGEHOG.ToString(), 
                                SqType.RABBIT.ToString(),  SqType.CARROT.ToString(), SqType.SQ_4.ToString(), 
                                SqType.SQ_3.ToString(), SqType.SQ_2.ToString(), SqType.HEDGEHOG.ToString(),
                                SqType.RABBIT.ToString(), SqType.SQ_156.ToString(),  SqType.CARROT.ToString(), 
                                SqType.HEDGEHOG.ToString(), SqType.SQ_2.ToString(), SqType.SQ_3.ToString(), 
                                SqType.HEDGEHOG.ToString(),  SqType.CARROT.ToString(), SqType.RABBIT.ToString(), 
                                SqType.CARROT.ToString(), SqType.SQ_2.ToString(), SqType.SALAD.ToString(), 
                                SqType.HEDGEHOG.ToString(), SqType.SQ_3.ToString(), SqType.SQ_4.ToString(), 
                                SqType.RABBIT.ToString(), SqType.SQ_2.ToString(), SqType.SQ_156.ToString(), 
                                SqType.CARROT.ToString(), SqType.HEDGEHOG.ToString(), SqType.RABBIT.ToString(), 
                                SqType.SQ_3.ToString(), SqType.SQ_2.ToString(), SqType.SQ_4.ToString(), 
                                SqType.CARROT.ToString(), SqType.HEDGEHOG.ToString(), SqType.SALAD.ToString(), 
                                SqType.RABBIT.ToString(),  SqType.CARROT.ToString(), SqType.SQ_2.ToString(),
                                SqType.RABBIT.ToString(), SqType.SALAD.ToString(), SqType.RABBIT.ToString()}; //first square is 1

    public Board(Player[] curPlayers){
        players = curPlayers;
        positions = new bool[65]; //63 squares and 64 as the finish (whether a player is at 0 does not matter)
        curPlayer = 0; //the starting player is 0
        numWinners = 1; //to get home first you need <= 10 carrots
    }

    public static int cost(int start, int end){ //calculates the cost of a move from start/end positions
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
    public static bool isOccupied(int pos){
        return positions[pos];
    }

    //returns the order of the players on the board
    //order[0] is the player in first place
    public static Player[] playerOrder(){
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

    public static void updateCurPlayer(){
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
    }

    //checks if the player currently moving passes a rabbit square with another player
    //and increments numPassed for that player
    //direction is moving forwards (true) or backwards (false)
    public static void checkPassing(int startPos, int endPos, bool dir){
        if(dir){
            for(int i = startPos; i < endPos; i++){
                //if passing a rabbit square
                if(squares[i] == SqType.RABBIT.ToString()){
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
                if(squares[i] == SqType.RABBIT.ToString()){
                    foreach(Player otherPlayer in players){
                        if(otherPlayer.pos == i){
                            otherPlayer.numPassed++;
                        }
                    }
                }
            }
        }
    }

    //makes the current player move to position pos and returns true if legal
    public bool move(int newPos) {
        Player p = players[curPlayer];
        int c = cost(p.pos, newPos);

        //awarding carrots if on a number square
        Player[] order = playerOrder();
        int place = 0;
        for(int i = 0; i < order.Length; i++){
            if(p == order[i]){
                place = i;
            }
        }
        string curSquare = squares[p.pos];
        if(curSquare == SqType.SQ_156.ToString() || curSquare == SqType.SQ_2.ToString() 
                || curSquare == SqType.SQ_3.ToString() || curSquare == SqType.SQ_4.ToString()){
            if(curSquare == SqType.SQ_156.ToString()){
                if(place == 1 || place == 5 || place == 6){
                    p.carrots += place * 10;
                }
            }
            else{ //square is either 2, 3, or 4
                if(int.Parse(curSquare[curSquare.Length - 1].ToString()) == place){
                    p.carrots += place * 10;
                }
            }
        }

        //awarding/giving carrots for rabbit squares
        if(p.numPassed == 0){ //no players passed the current player
            p.carrots += 10; //get 10 carrots
        }
        else{
            p.carrots -= 10 * p.numPassed;
        }
        p.numPassed = 0;

        if(newPos == p.pos){ //staying in the same spot (only on rabbit and carrot spaces)
            if(squares[newPos] == SqType.RABBIT.ToString() || squares[newPos] == SqType.CARROT.ToString()){
                if(squares[newPos] == SqType.RABBIT.ToString()){ 
                    p.carrots += 10;
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
            if(c < p.carrots && squares[newPos] != SqType.HEDGEHOG.ToString() && !isOccupied(newPos)){
                if(squares[newPos] == SqType.SALAD.ToString()){
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
            if(squares[newPos] == SqType.HEDGEHOG.ToString() && !isOccupied(newPos)){
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