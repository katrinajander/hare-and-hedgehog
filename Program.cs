using System;
using static System.Console;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

delegate void Notify();

class MyWindow : Form{
    Game game = new Game(0); //initialize the game

    bool runHome = true;
    bool runPlayers = false;
    bool setup = true;

    int MARGIN = 50; //50 px
    int SQUARE_LENGTH = 60; //square side length
    int HALF_SQUARE = 30;
    int BOARD_LENGTH = 900;
    int BOARD_HEIGHT = 660;

    //locations of squares on the board
    Point[] points = {new Point(12,10), new Point(12, 12), new Point(10,12), new Point(9,14), new Point(9,16), 
                    new Point(11,17), new Point(11,19), new Point(9,19), new Point(7,20), new Point(5,20), 
                    new Point(3,20), new Point(1,19), new Point(0,17), new Point(0,15), new Point(0,13), 
                    new Point(2,13), new Point(4,13), new Point(4,11), new Point(4,9), new Point(2,9), 
                    new Point(0,8), new Point(0,6), new Point(0,4), new Point(1,2), new Point(2,0), 
                    new Point(4,0), new Point(6,0), new Point(7,2), new Point(9,3), new Point(11,3), 
                    new Point(11,1), new Point(13,1), new Point(15,1), new Point(17,1), new Point(17,3), 
                    new Point(19,3), new Point(21,2), new Point(22,0), new Point(24,0), new Point(26,0), 
                    new Point(27,2), new Point(28,4), new Point(28,6), new Point(28,8), new Point(26,9), 
                    new Point(24,9), new Point(24,11), new Point(24,13), new Point(26,13), new Point(28,13), 
                    new Point(28,15), new Point(28,17), new Point(27,19), new Point(25,20), new Point(24,18), 
                    new Point(23,16), new Point(21,16), new Point(20,18), new Point(20,20), new Point(18,20), 
                    new Point(16,19), new Point(16,17), new Point(16,15), new Point(16,13), new Point(16,11)};
    
    Point[][] boxes = {new Point[] {new Point(50,450), new Point(210,525)}, 
                        new Point[] {new Point(235,450), new Point(395,525)},
                        new Point[] {new Point(420,450), new Point(580,525)},
                        new Point[] {new Point(605,450), new Point(765,525)},
                        new Point[] {new Point(790,450), new Point(950,525)}};
    
    Brush[] brushes = {Brushes.Red, Brushes.Green, Brushes.Blue, 
                        Brushes.Orange, Brushes.White, Brushes.Purple};
    Brush textBrush = Brushes.Black;
    Point[] playerTextLoc = {new Point(250,240), new Point(250,270), new Point(250,300),
                        new Point(520,240), new Point(520,270), new Point(520,300)};
    Font titleFont = new Font("Verdana", 20.0f);
    Font playerFont = new Font("Verdana", 12.0f);

    //bitmaps
    static Bitmap boardImage = new Bitmap("testboard.jpg");
    static Bitmap homeImage = new Bitmap("homescreenscale.jpg");
    static Bitmap playersImage = new Bitmap("playersscreenscale.jpg");

    public MyWindow() {
        Text = "Hare and Hedgehog";
        ClientSize = new Size(1000, 760);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        DoubleBuffered = true;
    }

    //returns the int position of which square was clicked
    public int clickedSquare(int x, int y){
        x -= MARGIN;
        y -= MARGIN;
        if(x < 0 || y < 0 || x > BOARD_LENGTH || y > BOARD_HEIGHT){
            return -1; //no square clicked
        }

        Point clickP = new Point(x / HALF_SQUARE, y / HALF_SQUARE); //map to a square

        for(int i = 0; i < points.Length; i++){
            if(clickP == points[i]){
                return i;
            }
        }
        //try x - 1, y - 1, and both x - 1, y - 1
        Point clickPX = new Point(clickP.X - 1, clickP.Y);
        Point clickPY = new Point(clickP.X, clickP.Y - 1);
        Point clickPXY = new Point(clickP.X - 1, clickP.Y - 1);
        for(int i = 0; i < points.Length; i++){
            if(clickPX == points[i] || clickPY == points[i] || clickPXY == points[i]){
                return i;
            }
        }
        return -1;
    }

    protected override void OnMouseDown(MouseEventArgs args) {
        int x = args.Location.X;
        int y = args.Location.Y;
        if(setup == false){ //during regular game
            int square = clickedSquare(x,y);
            if(square > 0){
                game.b.move(square);
            }
            Invalidate();
        }
        else{ //during setup, want to move to next screen
            if(runHome){
                Invalidate();
                runHome = false;
                runPlayers = true;
            }
            else if(runPlayers){
                int numPlayers = 0;
                int i = 2;
                foreach(Point[] p in boxes){
                    if(x > p[0].X && x < p[1].X && y > p[0].Y && y < p[1].Y){
                        numPlayers = i;
                    }
                    i++;
                }
                
                if(numPlayers >= 2 && numPlayers <= 6){
                    game.numPlayers = numPlayers;
                    game.setArray(numPlayers);
                    Invalidate();
                    runPlayers = false;
                    setup = false;
                }
            }
            else{
                setup = false;
            }
        }
    }

    static void homeScreen(PaintEventArgs args, Bitmap homeImage){
        Graphics g = args.Graphics;
        g.DrawImage(homeImage, 0, 0);
    }
    static void playersScreen(PaintEventArgs args){
        Graphics g = args.Graphics;
        g.DrawImage(playersImage, 0, 0);
    }

    protected override void OnPaint(PaintEventArgs args) {
        if(!setup){
            Graphics g = args.Graphics;
            g.DrawImage(boardImage, 0, 0);
            g.DrawString("Hare and Hedgehog", titleFont, textBrush, new Point(360,25));
            g.DrawString("Current player: " + Board.players[Board.curPlayer].color, playerFont, textBrush, new Point(710,25));
            g.DrawString("Cost to move forward is n(n+1)/2, where n = # of spaces", playerFont, textBrush, new Point(25,735));

            foreach(Player p in Board.players){
                int playerPos = p.pos;
                if(playerPos == 0 || playerPos == 64){ //start or end
                    g.FillEllipse(brushes[p.num], MARGIN + points[playerPos].X * 30 + p.num * 10, 
                    MARGIN + points[playerPos].Y * 30, SQUARE_LENGTH, SQUARE_LENGTH);
                }
                else{
                    if(p == Board.players[Board.curPlayer]){
                        g.FillEllipse(Brushes.White, MARGIN + points[playerPos].X * 30 + HALF_SQUARE / 2, 
                        MARGIN + points[playerPos].Y * 30 + HALF_SQUARE / 2, HALF_SQUARE, HALF_SQUARE);
                    }
                    else{
                        g.FillEllipse(Brushes.Black, MARGIN + points[playerPos].X * 30 + HALF_SQUARE / 2, 
                        MARGIN + points[playerPos].Y * 30 + HALF_SQUARE / 2, HALF_SQUARE, HALF_SQUARE);
                    }
                    g.FillEllipse(brushes[p.num], MARGIN + points[playerPos].X * 30 + (HALF_SQUARE + 10) / 2, 
                    MARGIN + points[playerPos].Y * 30 + (HALF_SQUARE + 10)/ 2, HALF_SQUARE - 10, HALF_SQUARE -10);
                }
                g.DrawString(p.color + ": " + p.carrots + " carrots, " + p.salads + " salads", 
                    playerFont, textBrush, playerTextLoc[p.num]);
            }

        }
        
        else{ //before the game starts, need player input/instruction screen
            if(runHome){
                homeScreen(args, homeImage);
            }
            if(runPlayers){
                playersScreen(args);
            }
        }
    }
}

static class Program{

    [STAThread]
    static void Main() {
        Form form = new MyWindow();
        Application.Run(form);
    }
}