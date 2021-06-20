class Game{
    string[] colors = {"Red", "Green", "Blue", "Orange", "White", "Purple"};
    public static Player[] p;
    public int numPlayers;
    public Board b;

    public void setArray(int numPlayers){
        p = new Player[numPlayers];
        for(int i = 0; i < numPlayers; i++){
            string name = "Player " + i;
            p[i] = new Player(i,colors[i]);
        }
        b = new Board(p);
    }

    public Game(int players){
        numPlayers = players;
        b = new Board(p);
    }
}