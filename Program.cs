using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace AmigaRL;

internal static class Program
{
    private const int SCALE = 6;
    private const int SCREEN_WIDTH = 320;
    private const int SCREEN_HEIGHT = 200;
    private const int CENTRE_X = SCREEN_WIDTH / 2;
    private const int CENTRE_Y = SCREEN_HEIGHT / 2;
    private const int WINDOW_WIDTH = SCREEN_WIDTH * SCALE;
    private const int WINDOW_HEIGHT = SCREEN_HEIGHT * SCALE;

    private static RenderTexture2D framebuffer;
    private static SpriteFont font;
    private static Texture2D copperUp;
    private static Texture2D copperDown;
    private static Texture2D floor;
    private static Texture2D background;
    private static Rectangle framebufferSrcRect = new(0, 0, SCREEN_WIDTH, -SCREEN_HEIGHT);
    private static Rectangle framebufferDstRect = new(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
    private static readonly CopperBar[] copperBars = new CopperBar[2];

    private static void Main()
    {
        // Strings
        const string text = "Amiga Style Scroller with Raylib";
        const string name = "Spec-Chum";

        // Init Raylib
        InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Amiga Style Demo");
        SetTargetFPS(60);

        framebuffer = LoadRenderTexture(SCREEN_WIDTH, SCREEN_HEIGHT);
        font = new(LoadTexture("font.png"), 32, 25, 48);

        InitialiseCopperBars();
        InitialiseTextures();

        int scrollOffset = SCREEN_WIDTH + 32;
        float theta = 0;

        while (!WindowShouldClose())
        {
            // Draw to texture (our framebuffer)
            BeginTextureMode(framebuffer);
            ClearBackground(Color.BLACK);

            font.WasSmallChar = false;
            DrawBackground(theta);
            DrawName(name, theta);
            DrawFloor();
            DrawCopperBars(copperDown, 1);
            DrawScroller(text, scrollOffset, theta);
            DrawCopperBars(copperUp, -1);
            UpdateCopperBars();

            EndTextureMode();

            // Blit framebuffer to screen
            BeginDrawing();
            BlitFramebuffer();
            EndDrawing();

            theta += 0.08f;

            scrollOffset--;
            if (scrollOffset < (32 * -text.Length))
            {
                scrollOffset = SCREEN_WIDTH + 32;
            }
        }

        Quit();
    }

    private static void InitialiseTextures()
    {
        Image image = GenImageGradientV(SCREEN_WIDTH, CENTRE_Y - 35, Color.BLACK, Color.DARKGREEN);
        floor = LoadTextureFromImage(image);

        image = GenImageChecked(SCREEN_WIDTH * 2, SCREEN_HEIGHT, 16, 16, Color.BLACK, Color.DARKGREEN);
        background = LoadTextureFromImage(image);

        UnloadImage(image);
    }

    private static void InitialiseCopperBars()
    {
        Image image = GenImageGradientV(SCREEN_WIDTH, 4, Color.RED, Color.BLACK);
        copperUp = LoadTextureFromImage(image);

        image = GenImageGradientV(SCREEN_WIDTH, 4, Color.BLACK, new Color(128, 0, 0, 255));
        copperDown = LoadTextureFromImage(image);

        copperBars[0] = new(CENTRE_Y - 40, CENTRE_Y + 32, CENTRE_Y - 40, 1);
        copperBars[1] = new(CENTRE_Y + 32, CENTRE_Y + 32, CENTRE_Y - 40, -1);

        UnloadImage(image);
    }

    private static void DrawBackground(float theta)
    {
        int x = (int)(32 * MathF.Cos(theta * 0.6f) - 32);
        int y = (int)(32 * MathF.Sin(theta * 0.6f) - 32);
        DrawTexture(background, x, y, Color.WHITE);
    }

    private static void DrawName(string name, float theta)
    {
        int smallCharOffset = 0;

        for (int i = 0; i < name.Length; i++)
        {
            smallCharOffset += font.WasSmallChar ? 10 : 0;
            Rectangle srcRect = font[name[i]];
            smallCharOffset += font.WasSmallChar ? 10 : 0;

            Rectangle dstRect = new(32 + (32 * i) - smallCharOffset + 10 * MathF.Cos(0.6f * (theta + i)), 25 + 10 * MathF.Sin(0.6f * (theta + i)), font.CellWidth, font.CellHeight);
            DrawTexturePro(font.texture, srcRect, dstRect, font.Origin, 0, new(0, 80, 0, 128));
        }
    }

    private static void DrawFloor()
    {
        DrawTexture(floor, 0, (SCREEN_HEIGHT / 2) + 35, Color.WHITE);
    }

    private static void DrawCopperBars(Texture2D texture, int direction)
    {
        for (int i = 0; i < copperBars.Length; i++)
        {
            ref CopperBar bar = ref copperBars[i];
            if (bar.Direction == direction)
            {
                bar.Draw(texture);
            }
        }
    }

    private static void DrawScroller(string text, int scrollOffset, float theta)
    {
        int smallCharOffset = 0;

        for (int i = 0; i < text.Length; i++)
        {
            smallCharOffset += font.WasSmallChar ? 10 : 0;
            Rectangle srcRect = font[text[i]];
            smallCharOffset += font.WasSmallChar ? 10 : 0;

            float sine = 20 * MathF.Sin(theta + i);
            float rotation = 20 * MathF.Sin(MathF.PI / 2 + theta + i);

            // Shadow
            Rectangle dstRect = new(scrollOffset - 8 + (32 * i) - smallCharOffset, (SCREEN_HEIGHT / 2) + sine - 8, font.CellWidth, font.CellHeight);
            DrawTexturePro(font.texture, srcRect, dstRect, font.Origin, rotation, new Color(0, 0, 0, 128));

            // Scroller
            dstRect = new(scrollOffset + (32 * i) - smallCharOffset, (SCREEN_HEIGHT / 2) + sine, font.CellWidth, font.CellHeight);
            DrawTexturePro(font.texture, srcRect, dstRect, font.Origin, rotation, Color.WHITE);

            // Reflection
            srcRect.height = -srcRect.height;
            dstRect = new(scrollOffset + (32 * i) - smallCharOffset, 70 + (SCREEN_HEIGHT / 2) - sine, font.CellWidth, font.CellHeight);
            DrawTexturePro(font.texture, srcRect, dstRect, font.Origin, -rotation, new Color(255, 255, 255, 32));
        }
    }

    private static void UpdateCopperBars()
    {
        for (int i = 0; i < copperBars.Length; i++)
        {
            ref CopperBar bar = ref copperBars[i];
            bar.Update();
        }
    }

    private static void BlitFramebuffer()
    {
        DrawTexturePro(framebuffer.texture, framebufferSrcRect, framebufferDstRect, Vector2.Zero, 0, Color.WHITE);
    }

    private static void Quit()
    {
        font.Unload();

        UnloadTexture(copperUp);
        UnloadTexture(copperDown);
        UnloadTexture(background);
        UnloadTexture(floor);
        UnloadRenderTexture(framebuffer);

        CloseWindow();
    }
}