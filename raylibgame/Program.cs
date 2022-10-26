using Raylib_cs;

string scene = "start";

Raylib.InitWindow(800, 640, "game");
Raylib.SetTargetFPS(60);

int defaultSpeed = 5;
int speed = defaultSpeed;
int crouchSpeed = 2;
int JumpStrength = 10;
int ys = 50;
int xs = 50;
float gravity = 0.5f;

int h = 0;

int block = 0;

bool doublejump = true;
bool isOnGround = true;
bool collideY = false;
bool collideX = false;

bool edited = false;
bool map = false;

int mx;
int my;

int mi = 0;
int mj = 0;

float s = 0;

Rectangle Player = new Rectangle(0, 0, xs, ys);
Player.x = 200;
Player.y = 100;

System.Numerics.Vector2 vel;
vel.Y = 0;
vel.X = 0;

Texture2D playerImg = Raylib.LoadTexture("player.png");
playerImg.width = xs;
playerImg.height = ys;

Color mapbg = new Color(100, 100, 100, 100);

int floor = 0;
int room = 2;

int[,] editlvl = { { 0 } };

//platform  //speed     //launch      //bounce     //quicksand   //conveyor L    //conveyor R
Color[] levelColor = new Color[] { Color.BLACK, Color.BLUE, Color.GREEN, Color.ORANGE, Color.LIME, Color.YELLOW, Color.DARKGRAY, Color.DARKGRAY };
//1         //2          //3           //4         //5           //6             //7

Array[,] levels1 = new Array[8, 5];
for (var i = 0; i < 8; i++)
{
    for (var j = 0; j < 5; j++)
    {
        levels1[i, j] = readLevel($"level{j}_{i}");
    }
}


Raylib.SetExitKey(KeyboardKey.KEY_NULL);
while (!Raylib.WindowShouldClose())
{
    mx = Raylib.GetMouseX();
    my = Raylib.GetMouseY();
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);

    if (scene == "game")
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_R)) { Player.x = 200; Player.y = 400; floor = 3; room = 2; }
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_ESCAPE)) { scene = "start"; map = false; }
        checkFloorRoom();
        drawLevel((int[,])levels1[floor, room]); //also handles collision detection
        PlayerMove();
        drawPlayer();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
        {
            map = !map;
        }
    }
    else if (scene == "start")
    {
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_ESCAPE))
        {
            scene = "game";
        }
        if (mx > 50 && mx < 150)
        {
            if (my > 250 && my < 350)
            {
                Raylib.DrawRectangle(50, 300, 70, 25, Color.GRAY);
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    scene = "game";
                }
            }

            else if (my > 350 && my < 450)
            {
                Raylib.DrawRectangle(50, 400, 150, 25, Color.GRAY);
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    scene = "editmap";
                }
            }

            else if (my > 450 && my < 550)
            {
                Raylib.DrawRectangle(50, 500, 50, 25, Color.GRAY);
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    break;
                }
            }
        }
        Raylib.DrawText("Start", 50, 300, 24, Color.WHITE);
        Raylib.DrawText("Level editor", 50, 400, 24, Color.WHITE);
        Raylib.DrawText("Quit", 50, 500, 24, Color.WHITE);

        Raylib.DrawText("The Game of All Time", 50, 100, (int)(Math.Abs(Math.Sin(s)) * 20) + 10, Color.WHITE);
        s += 0.01f;
    }
    else if (scene == "editmap")
    {
        Raylib.DrawRectangle(0, 0, 100, 50, Color.RED);
        if (mx > 0 && mx < 100 && my > 0 && my < 50)
        {
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
            {
                scene = "start";
            }
        }
        for (var i = 1; i < 6; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (mx > i * 100 + 50 && mx < i * 100 + 150 && my > j * 80 && my < j * 80 + 80)
                {
                    Raylib.DrawRectangle(i * 100 + 50, j * 80 - 4, 100, 80, Color.DARKGRAY);
                    if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        mi = i - 1;
                        mj = 7 - j;
                        scene = "edit";
                    }
                }
            }
        }
        for (var i = 0; i < 5; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                drawMiniLevel((int[,])levels1[j, i], i, -j, 4);
            }
        }
    }
    else if (scene == "edit")
    {
        Raylib.DrawRectangle(100, 75, 600, 480, mapbg);
        drawMiniLevel((int[,])levels1[mj, mi], -0.5, -6, 24);
        editlvl = (int[,])levels1[mj, mi];
        Raylib.DrawRectangle(0, 0, 50, 50, Color.RED);
        if (mx < 50 && my < 50)
        {
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
            {
                edited = false;
                levels1[mj, mi] = readLevel($"level{mi}_{mj}");
                scene = "editmap";
            }
        }
        if (edited)
        {
            Raylib.DrawRectangle(50, 0, 50, 50, Color.GREEN);
            if (mx > 50 && mx < 100 && my < 50)
            {
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    levels1[mj, mi] = editlvl;
                    writeLevel((int[,])levels1[mj, mi], mi, mj);
                    scene = "editmap";
                    edited = false;
                }
            }
        }
        for (var i = 4; i < 29; i++)
        {
            for (var j = 3; j < 23; j++)
            {
                if (mx > i * 24 + 4 && mx < i * 24 + 28 && my > j * 24 + 3 && my < j * 24 + 27)
                {
                    Rectangle recLine = new Rectangle(i * 24 + 4, j * 24 + 3, 24, 24);
                    Raylib.DrawRectangleLinesEx(recLine, 5, levelColor[block]);
                    if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        editlvl[j - 3, i - 4] = block;
                        edited = true;
                    }
                }
            }
        }
        for (var i = 0; i < 8; i++)
        {
            Raylib.DrawRectangle(i * 50, Raylib.GetScreenHeight() - 50, 50, 50, levelColor[i]);
            if (mx > i * 50 && mx < i * 50 + 50 && my > Raylib.GetScreenHeight() - 50)
            {
                Rectangle recLine = new Rectangle(i * 50, Raylib.GetScreenHeight() - 50, 50, 50);
                Raylib.DrawRectangleLinesEx(recLine, 5, Color.WHITE);
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    block = i;
                }
            }
        }
    }
    if (map)
    {
        speed = crouchSpeed;
        Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenWidth(), mapbg);
        for (var i = 0; i < 5; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                drawMiniLevel((int[,])levels1[j, i], i, -j, 4);
            }
        }
        Raylib.DrawRectangle((int)(Player.x / 8) + 100 * room + 150, Raylib.GetScreenHeight() - (int)(Player.y / 8) - 80 * floor - 12, 8, 8, Color.RED);
    }
    else { speed = defaultSpeed; }
    Raylib.EndDrawing();
}


void checkFloorRoom()
{
    if (Player.y + ys / 2 > Raylib.GetScreenHeight())
    {
        floor++;
        Player.y = -ys / 2;
    }
    else if (Player.y + ys / 2 < -ys)
    {
        floor--;
        Player.y = Raylib.GetScreenHeight() - ys / 2;
    }

    if (Player.x + xs / 2 > Raylib.GetScreenWidth())
    {
        room++;
        Player.x = 0 - xs / 2;
    }
    else if (Player.x + xs / 2 < 0)
    {
        room--;
        Player.x = Raylib.GetScreenWidth() - xs / 2;
    }
    if (room > levels1.GetLength(1) - 1) { room = 0; }
    else if (room < 0) { room = levels1.GetLength(1) - 1; }
}

void drawPlayer()
{
    // Raylib.DrawRectangle((int)(Player.x + vel.X), Raylib.GetScreenHeight() - (int)Player.y - (int)vel.Y - ys, xs, ys, Color.RED);
    Raylib.DrawTexture(playerImg, (int)Player.x, Raylib.GetScreenHeight() - (int)Player.y - ys, Color.RED);
    // Raylib.DrawRectangle((int)Player.x, Raylib.GetScreenHeight() - (int)Player.y - ys, xs, ys, rgb());
}

void PlayerMove()
{
    if (collideY)
    {
        isOnGround = true;
    }
    else
    {
        isOnGround = false;
        vel.Y -= gravity;
        if (vel.Y < 0) { vel.Y -= gravity / 2; }
    }

    if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { ys = 25; speed = crouchSpeed; playerImg.height = ys; }
    else { ys = 50; playerImg.height = ys; }

    vel.X += (Raylib.IsKeyDown(KeyboardKey.KEY_D) - Raylib.IsKeyDown(KeyboardKey.KEY_A)) * speed;
    vel.X *= 0.5f;

    if (Raylib.IsKeyPressed(KeyboardKey.KEY_W) || Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
    {
        if (isOnGround)
        {
            vel.Y = JumpStrength;
        }
        else if (doublejump)
        {
            vel.Y = JumpStrength;
            doublejump = false;
        }
    }

    if (!collideX) { Player.x += vel.X; } else { collideX = false; }
    if (!collideY) { Player.y += vel.Y; } else { collideY = false; if (vel.Y < 0) { vel.Y = 0; } }
}

void drawLevel(int[,] level)
{
    for (var i = 0; i < level.GetLength(0); i++)
    {
        for (var j = 0; j < level.GetLength(1); j++)
        {
            if (level[i, j] != 0)
            {
                if (i == 20) { Raylib.DrawRectangle(j * 32, i * 32 - 2, 32, 32, levelColor[level[i, j]]); }
                else { Raylib.DrawRectangle(j * 32, i * 32, 32, 32, levelColor[level[i, j]]); }
                if (Player.x + vel.X + xs > j * 32 && Player.x + vel.X < j * 32 + 32 && Raylib.GetScreenHeight() - Player.y - vel.Y - 1 > i * 32 && Raylib.GetScreenHeight() - Player.y - vel.Y - ys < i * 32 + 32)
                {
                    if (vel.X > 0)
                    {
                        collideX = true;
                        vel.X = 0;
                    }
                    else
                    {
                        collideX = true;
                        vel.X = 0;
                    }
                }
                if (Raylib.GetScreenHeight() - Player.y - vel.Y > i * 32 && Raylib.GetScreenHeight() - Player.y - vel.Y - ys < i * 32 + 32 && Player.x + vel.X + xs - 1 > j * 32 && Player.x + vel.X + 1 < j * 32 + 32)
                {
                    if (vel.Y > 0)
                    {
                        collideY = true;
                        vel.Y = 0;
                    }
                    else
                    {
                        collideY = true;
                        doublejump = true;
                        if (level[i, j] == 2) { speed = 10; }
                        if (level[i, j] == 3) { vel.Y = 15; }
                        if (level[i, j] == 4) { vel.Y = -vel.Y / 1.5f; }
                        if (level[i, j] == 5) { collideY = false; vel.Y = -0.01f; }
                        if (level[i, j] == 6) { vel.X += speed / 1.5f; }
                        if (level[i, j] == 7) { vel.X -= speed / 1.5f; }
                    }
                }
            }
        }
    }
}

void drawMiniLevel(int[,] level, double x, int y, int size)
{
    for (var i = 0; i < level.GetLength(0); i++)
    {
        for (var j = 0; j < level.GetLength(1); j++)
        {
            if (level[i, j] != 0)
            {
                Raylib.DrawRectangle(j * size + (int)(x * 100) + 150, i * size + y * 80 + 556, size, size, levelColor[level[i, j]]);
            }
        }
    }
}


static void writeLevel(int[,] level, int x, int y)
{
    var sw = new StreamWriter($"levels/level{x}_{y}");
    for (var i = 0; i < level.GetLength(0); i++)
    {
        for (var j = 0; j < level.GetLength(1); j++)
        {
            var c = level[i, j];
            sw.Write(level[i, j]);
        }
        sw.Write("\n");
    }
    sw.Flush();
    sw.Close();
}

static int[,] readLevel(string file)
{
    int[,] level = new int[21, 25];
    var sr = new StreamReader("levels/" + file);
    for (var i = 0; i < 21; i++)
    {
        string line = sr.ReadLine();
        char[] linearr = line.ToCharArray();
        for (var j = 0; j < 25; j++)
        {
            line = linearr[j].ToString();
            level[i, j] = int.Parse(line);
        }
    }
    sr.Close();
    return (level);
}

Color rgb()
{
    Color color = Raylib.ColorFromHSV(h, 1, 1);
    if (h < 360) { h += 2; } else { h = 0; }
    return (color);
}