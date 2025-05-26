using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class GameHeaderUI : CanvasObject
{
    private ButtonObject _unitInventoryButton;

    public const string BUTTONIDLERESOURCENAME = "GameHeaderButtonIdle";
    public const string BUTTONHOVEREDRESOURCENAME = "GameHeaderButtonHovered";
    public const int INVENTORYBUTTONPOSITIONX = 1000;
    public const int INVENTORYBUTTONPOSITIONY = 50;


    public GameHeaderUI() 
    {
        _unitInventoryButton = new ButtonObject(new DrawableResource_Polygon(MasterAssetLoader.GetPolygon(BUTTONIDLERESOURCENAME).Resource), //TODO
            new DrawableResource_Polygon(MasterAssetLoader.GetPolygon(BUTTONHOVEREDRESOURCENAME).Resource), //TODO
            null, 
            new Position(INVENTORYBUTTONPOSITIONX, INVENTORYBUTTONPOSITIONY));
        AddChild(_unitInventoryButton);
    }

}

