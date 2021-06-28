using System;
using static System.Console;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class Player{
    public int num;
    public string color;
    public int carrots; //how many carrots the player has
    public int salads; //how many salads they player has
    public int pos; //on which square the player is
    public bool won; //if the player has won already
    public bool eatingSalad; //if the player is currently eating salad
    public int numPassed; //number of players who passed the player (for rabbit squares)

    public Player(int playerNumber, string playerColor){
        num = playerNumber;
        color = playerColor;
        carrots = 65; //start with 65 carrots
        salads = 3; //start with 3 salads
        pos = 0; //start off the board on position 0
        won = false; //skip turn if won
        eatingSalad = false; //skip turn if eating salad
        numPassed = 0; //starts out empty
    }
}