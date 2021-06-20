class Board{

    public event Notify changed; // fires whenever the board changes

    public static Player[] players;
    public static bool[] positions; //true if there is a player at that position
    public static int curPlayer;
    public int numWinners; //to calculate how many carrots to finish with legally

    static string[] squares = {"", "rabbit", "carrot", "rabbit", "3", "carrot", "rabbit", "salad", "hedgehog", "4", "2",
                        "hedgehog", "3", "carrot", "rabbit", "hedgehog", "1,5,6", "2", "4", "hedgehog", "3",
                        "carrot", "salad", "2", "hedgehog", "rabbit", "carrot", "4", "3", "2", "hedgehog",
                        "rabbit", "1,5,6", "carrot", "hedgehog", "2", "3", "hedgehog", "carrot", "rabbit", "carrot",
                        "2", "salad", "hedgehog", "3", "4", "rabbit", "2", "1,5,6", "carrot", "hedgehog",
                        "rabbit", "3", "2", "4", "carrot", "hedgehog", "salad", "rabbit", "carrot", "2",
                        "rabbit", "salad", "rabbit"}; //first square is 1

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
            //one round has passed, check if anyone is on a number space and give carrots
            Player[] order = playerOrder();

            for(int i = 0; i < players.Length; i++){
                Player p = order[i];
                string type = squares[p.pos];
                if(type == "1,5,6" || type == "2" || type == "3" || type == "4"){ //if on a number square
                    if(type == "1,5,6"){
                        if(i == 1 || i == 5 || i == 6){
                            p.carrots += i * 10;
                        }
                    }
                    else{ //square is either 2, 3, or 4
                        if(int.Parse(type) == i){
                            p.carrots += i * 10;
                        }
                    }
                }
            }
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

    //makes the current player move to position pos and returns true if legal
    public bool move(int newPos) {
        Player p = players[curPlayer];
        int c = cost(p.pos, newPos);
        if(newPos == p.pos){ //staying in the same spot (only on rabbit and carrot spaces)
            if(squares[newPos] == "rabbit" || squares[newPos] == "carrot"){ //rabbit squares do nothing
                if(squares[newPos] == "rabbit"){ //FIX to be able to reduce carrots
                    p.carrots += 10;
                }
                updateCurPlayer();
                if (changed != null){
                    changed();
                }
                return true;
            }
        }
        else if(newPos > p.pos){ //moving forwards
            //if the player is crossing the finish line (space can be occupied)
            if(newPos == 64){ 
                if(p.carrots - c <= (numWinners * 10) && p.salads == 0){
                    p.won = true;
                }
            }
            //if the player can pay for the move, is not moving forward onto a hedgehog, and an empty space
            if(c < p.carrots && squares[newPos] != "hedgehog" && !isOccupied(newPos)){
                if(squares[newPos] == "salad"){
                    p.eatingSalad = true;
                    p.salads--; //eat a salad
                }
                p.carrots -= c; //spend the carrots
                positions[p.pos] = false;
                positions[newPos] = true; // moved to new spot
                p.pos = newPos;
                updateCurPlayer();
                if (changed != null){
                    changed();
                }
                return true;
            }
        }
        else{ //moving backwards
            if(squares[newPos] == "hedgehog" && !isOccupied(newPos)){
                p.carrots += c; //gain carrots
                positions[p.pos] = false;
                positions[newPos] = true; //moved to new spot
                p.pos = newPos;
                updateCurPlayer();
                if (changed != null){
                    changed();
                }
                return true;
            }
        }
        return false;
    }
}