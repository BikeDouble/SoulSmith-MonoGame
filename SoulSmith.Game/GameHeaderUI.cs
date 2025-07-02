using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Game;
public class GameHeaderUI : CanvasObject
{
    // Children
    private ButtonObject _unitInventoryButton;

    public const string BUTTONIDLERESOURCENAME = "GameHeaderButtonIdle";
    public const string BUTTONHOVEREDRESOURCENAME = "GameHeaderButtonHovered";
    public const int INVENTORYBUTTONPOSITIONX = 1000;
    public const int INVENTORYBUTTONPOSITIONY = 50;


    public GameHeaderUI() 
    {
        /*_unitInventoryButton = new ButtonObject(null, //TODO
            null, 
            new Position(INVENTORYBUTTONPOSITIONX, INVENTORYBUTTONPOSITIONY));
        AddChild(_unitInventoryButton);*/
    }

}

