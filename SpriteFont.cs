using Raylib_cs;
using System.Numerics;

namespace AmigaRL;

public class SpriteFont
{
    public readonly Texture2D texture;
    public readonly int CellWidth;
    public readonly int CellHeight;
    public Vector2 Origin;
    public bool WasSmallChar;

    private readonly List<Rectangle> letterRects;

    public SpriteFont(Texture2D image, int cellWidth, int cellHeight, int numCells = 0)
    {
        texture = image;
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        WasSmallChar = false;

        Origin = new(cellWidth / 2, cellHeight / 2);
        if (numCells == 0)
        {
            numCells = image.width / cellWidth * image.height / cellHeight;
        }
        letterRects = GenerateRects(numCells);
    }

    public Rectangle this[char letter] => letterRects[GetLetterIndex(letter)];

    public Rectangle GetLetterRect(char letter) => letterRects[GetLetterIndex(letter)];

    public int GetLetterIndex(char letter)
    {
        letter = char.ToUpper(letter);
        WasSmallChar = false;

        int letterIndex;
        if (char.IsDigit(letter))
        {
            letterIndex = letter - '0' + 30;
        }
        else
        {
            switch (letter)
            {
                case 'I':
                    letterIndex = 8;
                    WasSmallChar = true;
                    break;

                case '!':
                    letterIndex = 26;
                    WasSmallChar = true;
                    break;

                case '\'':
                    letterIndex = 46;
                    WasSmallChar = true;
                    break;

                case ' ':
                    letterIndex = 47;
                    //WasSmallChar = true;
                    break;

                case '?':
                    letterIndex = 27;
                    break;

                case ':':
                    letterIndex = 28;
                    break;

                case ';':
                    letterIndex = 29;
                    break;

                case '"':
                    letterIndex = 40;
                    break;

                case '(':
                    letterIndex = 41;
                    break;

                case ')':
                    letterIndex = 42;
                    break;

                case ',':
                    letterIndex = 43;
                    break;

                case '-':
                    letterIndex = 44;
                    break;

                case '.':
                    letterIndex = 45;
                    break;

                default:
                    letterIndex = letter - 'A';
                    break;
            }
        }

        return letterIndex;
    }

    public void Unload() => Raylib.UnloadTexture(texture);

    private List<Rectangle> GenerateRects(int numCells)
    {
        List<Rectangle> rects = new(numCells);
        for (int i = 0; i < texture.height / CellHeight; i++)
        {
            for (int j = 0; j < texture.width / CellWidth; j++)
            {
                rects.Add(new Rectangle(j * CellWidth, i * CellHeight, CellWidth, CellHeight));
                if (rects.Count == numCells)
                {
                    return rects;
                }
            }
        }

        return rects;
    }
}