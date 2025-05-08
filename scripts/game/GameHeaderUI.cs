using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameHeaderUI : CanvasItem
{
    private Button _unitInventoryButton;

    public const string BUTTONIDLERESOURCENAME = "GameHeaderButtonIdle";
    public const string BUTTONHOVEREDRESOURCENAME = "GameHeaderButtonHovered";
    public const int INVENTORYBUTTONPOSITIONX = 1000;
    public const int INVENTORYBUTTONPOSITIONY = 50;


    public GameHeaderUI() 
    {
        _unitInventoryButton = new Button(new DrawableResource_Polygon(AssetLoader.GetPolygon(BUTTONIDLERESOURCENAME)),
            new DrawableResource_Polygon(AssetLoader.GetPolygon(BUTTONHOVEREDRESOURCENAME)), 
            null, 
            new CanvasPosition(INVENTORYBUTTONPOSITIONX, INVENTORYBUTTONPOSITIONY));
        AddChild(_unitInventoryButton);
    }

}

