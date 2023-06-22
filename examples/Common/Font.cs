using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Breakout;
using Pie;
using Pie.Text;

namespace Common;

public sealed class Font : IDisposable
{
    private static FreeType _freeType;
    private uint _fontReferences;

    private Face _face;

    private Dictionary<uint, Dictionary<char, (Character, Texture)>> _characterDict;

    public Font(string path)
    {
        if (_freeType == null)
        {
            _freeType = new FreeType();
            _fontReferences++;
        }

        _face = _freeType.CreateFace(path, 0);
        _characterDict = new Dictionary<uint, Dictionary<char, (Character, Texture)>>();
    }

    public void Draw(SpriteRenderer renderer, uint size, string text, Vector2 position)
    {
        if (!_characterDict.TryGetValue(size, out Dictionary<char, (Character, Texture)> dict))
        {
            dict = new Dictionary<char, (Character, Texture)>();
            _characterDict.Add(size, dict);
        }

        foreach (char c in text)
        {
            if (!dict.TryGetValue(c, out (Character character, Texture texture) character))
            {
                SampleApplication.Log(LogType.Debug, $"Creating character '{c}' at size {size}.");
                _face.Size = (int) size;

                character.character = _face.Characters[c];
                if (character.character.Width > 0 && character.character.Height > 0)
                {
                    Size texSize = new Size(character.character.Width, character.character.Height);
                    character.texture =
                        Utils.CreateTexture2D(renderer.Device, new Bitmap(character.character.Bitmap, texSize));
                }

                dict.Add(c, character);
            }
        }
        
        int largestChar = 0;
        foreach (char c in text)
        {
            int bearing = dict[c].Item1.BitmapTop;
            if (bearing > largestChar)
                largestChar = bearing;
        }
        
        Vector2 pos = position;
        pos.Y += largestChar;

        foreach (char c in text)
        {
            if (c == '\n')
            {
                pos.Y += size;
                pos.X = position.X;
                continue;
            }
            
            (Character character, Texture texture) = dict[c];

            if (texture != null)
            {
                renderer.Draw(texture, new Vector2(pos.X + character.BitmapLeft, pos.Y - character.BitmapTop),
                    Color.White, 0, Vector2.One, Vector2.Zero);
            }

            pos.X += character.Advance;
        }
    }
    
    public void Dispose()
    {
        _face.Dispose();
        
        _fontReferences--;
        if (_fontReferences == 0)
            _freeType.Dispose();
    }
}