using Raylib_cs;
using static Raylib_cs.Raylib;

namespace AmigaRL;

public struct CopperBar
{
    public int Position;
    public int UpperLimit;
    public int LowerLimit;
    public int Direction;

    public CopperBar(int position, int upperLimit, int lowerLimit, int direction = 1)
    {
        Position = position;
        UpperLimit = upperLimit;
        LowerLimit = lowerLimit;
        Direction = direction;
    }

    public void Update()
    {
        Position += Direction;
        if (Position > UpperLimit || Position < LowerLimit)
        {
            Direction *= -1;
        }
    }

    public void Draw(Texture2D texture)
    {
        DrawTexture(texture, 0, Position, Color.WHITE);
    }
}