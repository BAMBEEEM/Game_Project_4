﻿/*
Squared.Tiled
Copyright (C) 2009 Kevin Gadd

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.

  Kevin Gadd kevin.gadd@gmail.com http://luminance.org/
*/
/*
 * Updates by Stephen Belanger - July, 13 2009
 * 
 * -added ProhibitDtd = false, so you don't need to remove the doctype line after each time you edit the map.
 * -changed everything to use SortedLists for easier referencing
 * -added objectgroups
 * -added movable and resizable objects
 * -added object images
 * -added meta property support to maps, layers, object groups and objects
 * -added non-binary encoded layer data
 * -added layer and object group transparency
 * 
 * TODO: I might add support for .tsx Tileset definitions. Note sure yet how beneficial that would be...
*/
/*
 * Modifications by Zach Musgrave - August 2012.
 * 
 * - Fixed errors in TileExample.cs
 * - Added support for rotated and flipped tiles (press Z, X, or Y in Tiled to rotate or flip tiles)
 * - Fixed exception when loading an object without a height or width attribute
 * - Fixed property loading bugs (properties now loaded for Layers, Maps, Objects)
 * - Added support for margin and spacing in tile sets
 * - CF-compatible System.IO.Compression library available via GitHub release. See releases at https://github.com/zachmu/tiled-xna
 * 
 * Zach Musgrave zach.musgrave@gmail.com http://gamedev.sleptlate.org
 */
/* Modifications by Nathan Bean - March 2022.
 * 
 * - Changed XMLReader settings to use DtdProcessing instead of now-depreciated ProhibitDtd = false 
 * - Added XML-style comments to each class and member
 * - Updated Example to use MonoGame 
 * 
 * Nathan Bean nhbean@ksu.edu
 */
/* Modifications by Ebraheem Mustafa - July 2024
 * 
 * - Added ability to scale tiles by providing float number
 * 
 * Ebraheem Mustafa ebraheem1@ksu.edu
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using SharpDX.Direct2D1.Effects;

namespace Game_Project_4
{
    /// <summary>
    /// A class representing a TileSet created with the Tiled map editor.
    /// </summary>
    public class Tileset
    {
        /// <summary>
        /// A class for holding a list of tile properties 
        /// </summary>
        /// <remarks>
        /// Essentially, a &lt;string, string&gt; dictionary
        /// </remarks>
        public class TilePropertyList : Dictionary<string, string>
        {
        }

        public string Name;
        public int FirstTileID;
        public int TileWidth;
        public int TileHeight;
        public int Spacing;
        public int Margin;
        public Dictionary<int, TilePropertyList> TileProperties = new Dictionary<int, TilePropertyList>();
        public string Image;
        protected Texture2D _Texture;
        protected int _TexWidth, _TexHeight;

        /// <summary>
        /// Loads a Tileset from a TMX file
        /// </summary>
        /// <param name="reader">A reader processing the file</param>
        /// <returns>An iniatlized Tileset object</returns>
        internal static Tileset Load(XmlReader reader)
        {
            var result = new Tileset();

            result.Name = reader.GetAttribute("name");
            result.FirstTileID = int.Parse(reader.GetAttribute("firstgid"));
            result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
            result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
            int.TryParse(reader.GetAttribute("margin"), out result.Margin);
            int.TryParse(reader.GetAttribute("spacing"), out result.Spacing);

            int currentTileId = -1;

            while (reader.Read())
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "image":
                                result.Image = reader.GetAttribute("source");
                                break;
                            case "tile":
                                currentTileId = int.Parse(reader.GetAttribute("id"));
                                break;
                            case "property":
                                {
                                    TilePropertyList props;
                                    if (!result.TileProperties.TryGetValue(currentTileId, out props))
                                    {
                                        props = new TilePropertyList();
                                        result.TileProperties[currentTileId] = props;
                                    }

                                    props[reader.GetAttribute("name")] = reader.GetAttribute("value");
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the properties of the specified tile
        /// </summary>
        /// <param name="index">The index of the tile</param>
        /// <returns>A TilePropertyList for the tile</returns>
        public TilePropertyList GetTileProperties(int index)
        {
            index -= FirstTileID;

            if (index < 0)
                return null;

            TilePropertyList result = null;
            TileProperties.TryGetValue(index, out result);

            return result;
        }

        /// <summary>
        /// Gets the texture of this Tileset
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value;
                _TexWidth = value.Width;
                _TexHeight = value.Height;
            }
        }

        /// <summary>
        /// Converts a map position into a rectangle providing the
        /// bounds of the tile in the TileSet texture.
        /// </summary>
        /// <param name="index">The tile index</param>
        /// <param name="rect">The bounds of the tile in the tileset texture</param>
        /// <returns>True if the tile index exists in the tileset</returns>
        internal bool MapTileToRect(int index, ref Rectangle rect)
        {
            index -= FirstTileID;

            if (index < 0)
                return false;

            int rowSize = _TexWidth / (TileWidth + Spacing);
            int row = index / rowSize;
            int numRows = _TexHeight / (TileHeight + Spacing);
            if (row >= numRows)
                return false;

            int col = index % rowSize;

            rect.X = col * TileWidth + col * Spacing + Margin;
            rect.Y = row * TileHeight + row * Spacing + Margin;
            rect.Width = TileWidth;
            rect.Height = TileHeight;
            return true;
        }
    }

    /// <summary>
    /// A class representing a Tiled map layer
    /// </summary>
    public class Layer
    {
        /*
         * High-order bits in the tile data indicate tile flipping
         */
        private const uint FlippedHorizontallyFlag = 0x80000000;
        private const uint FlippedVerticallyFlag = 0x40000000;
        private const uint FlippedDiagonallyFlag = 0x20000000;

        internal const byte HorizontalFlipDrawFlag = 1;
        internal const byte VerticalFlipDrawFlag = 2;
        internal const byte DiagonallyFlipDrawFlag = 4;

        public SortedList<string, string> Properties = new SortedList<string, string>();
        internal struct TileInfo
        {
            public Texture2D Texture;
            public Rectangle Rectangle;
        }

        public string Name;
        public int Width, Height;
        public float Opacity = 1;
        public int[] Tiles;
        public byte[] FlipAndRotate;
        internal TileInfo[] _TileInfoCache = null;

        /// <summary>
        /// Loads the layer from a TMX file
        /// </summary>
        /// <param name="reader">A reader to the TMX file currenlty being processed</param>
        /// <returns>An initialized Layer object</returns>
        internal static Layer Load(XmlReader reader)
        {
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            var result = new Layer();

            if (reader.GetAttribute("name") != null)
            {
                result.Name = reader.GetAttribute("name");
            }
            if (reader.GetAttribute("width") != null)
            {
                result.Width = int.Parse(reader.GetAttribute("width"));
            }
            if (reader.GetAttribute("height") != null)
            {
                result.Height = int.Parse(reader.GetAttribute("height"));
            }
            if (reader.GetAttribute("opacity") != null)
            {
                result.Opacity = float.Parse(reader.GetAttribute("opacity"), NumberStyles.Any, ci);
            }
            result.Tiles = new int[result.Width * result.Height];
            result.FlipAndRotate = new byte[result.Width * result.Height];

            while (!reader.EOF)
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "data":
                                {
                                    if (reader.GetAttribute("encoding") != null)
                                    {
                                        var encoding = reader.GetAttribute("encoding");
                                        var compressor = reader.GetAttribute("compression");
                                        switch (encoding)
                                        {
                                            case "base64":
                                                {
                                                    int dataSize = (result.Width * result.Height * 4) + 1024;
                                                    var buffer = new byte[dataSize];
                                                    reader.ReadElementContentAsBase64(buffer, 0, dataSize);

                                                    Stream stream = new MemoryStream(buffer, false);
                                                    if (compressor == "gzip")
                                                        stream = new GZipStream(stream, CompressionMode.Decompress, false);
                                                    if (compressor == "zlib")
                                                        stream = new ZLibStream(stream, CompressionMode.Decompress, false);

                                                    using (stream)
                                                    using (var br = new BinaryReader(stream))
                                                    {
                                                        for (int i = 0; i < result.Tiles.Length; i++)
                                                        {
                                                            uint tileData = br.ReadUInt32();

                                                            // The data contain flip information as well as the tileset index
                                                            byte flipAndRotateFlags = 0;
                                                            if ((tileData & FlippedHorizontallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= HorizontalFlipDrawFlag;
                                                            }
                                                            if ((tileData & FlippedVerticallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= VerticalFlipDrawFlag;
                                                            }
                                                            if ((tileData & FlippedDiagonallyFlag) != 0)
                                                            {
                                                                flipAndRotateFlags |= DiagonallyFlipDrawFlag;
                                                            }
                                                            result.FlipAndRotate[i] = flipAndRotateFlags;

                                                            // Clear the flip bits before storing the tile data
                                                            tileData &= ~(FlippedHorizontallyFlag |
                                                                          FlippedVerticallyFlag |
                                                                          FlippedDiagonallyFlag);
                                                            result.Tiles[i] = (int)tileData;
                                                        }
                                                    }

                                                    continue;
                                                };

                                            default:
                                                throw new Exception("Unrecognized encoding.");
                                        }
                                    }
                                    else
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            int i = 0;
                                            while (!st.EOF)
                                            {
                                                switch (st.NodeType)
                                                {
                                                    case XmlNodeType.Element:
                                                        if (st.Name == "tile")
                                                        {
                                                            if (i < result.Tiles.Length)
                                                            {
                                                                result.Tiles[i] = int.Parse(st.GetAttribute("gid"));
                                                                i++;
                                                            }
                                                        }

                                                        break;
                                                    case XmlNodeType.EndElement:
                                                        break;
                                                }

                                                st.Read();
                                            }
                                        }
                                    }
                                    Console.WriteLine("It made it!");
                                }
                                break;
                            case "properties":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        while (!st.EOF)
                                        {
                                            switch (st.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    if (st.Name == "property")
                                                    {
                                                        if (st.GetAttribute("name") != null)
                                                        {
                                                            result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                        }
                                                    }

                                                    break;
                                                case XmlNodeType.EndElement:
                                                    break;
                                            }

                                            st.Read();
                                        }
                                    }
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        /// <summary>
        /// Gets the tile index of the tile at position (<paramref name="x"/>,<paramref name="y"/>)
        /// in the layer
        /// </summary>
        /// <param name="x">The tile's x-position in the layer</param>
        /// <param name="y">The tile's y-position in the layer</param>
        /// <returns>The index of the tile in the tileset(s)</returns>
        public int GetTile(int x, int y)
        {
            if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height))
                throw new InvalidOperationException();

            int index = (y * Width) + x;
            return Tiles[index];
        }

        /// <summary>
        /// Caches the information about each specific tile in the layer
        /// (its texture and bounds within that texture) in a list indexed 
        /// by the tile index for quick retreival/processing
        /// </summary>
        /// <param name="tilesets">The list of tilesets containing tiles to cache</param>
        protected void BuildTileInfoCache(IList<Tileset> tilesets)
        {
            Rectangle rect = new Rectangle();
            var cache = new List<TileInfo>();
            int i = 1;

        next:
            for (int t = 0; t < tilesets.Count; t++)
            {
                if (tilesets[t].MapTileToRect(i, ref rect))
                {
                    cache.Add(new TileInfo
                    {
                        Texture = tilesets[t].Texture,
                        Rectangle = rect
                    });
                    i += 1;
                    goto next;
                }
            }

            _TileInfoCache = cache.ToArray();
        }

        /// <summary>
        /// Draws the layer
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw with</param>
        /// <param name="tilesets">A list of tilesets associated with the layer</param>
        /// <param name="rectangle">The viewport to render within</param>
        /// <param name="viewportPosition">The viewport's position in the layer</param>
        /// <param name="tileWidth">The width of a tile</param>
        /// <param name="tileHeight">The heiht of a tile</param>
        public void Draw(SpriteBatch batch, IList<Tileset> tilesets, Rectangle rectangle, Vector2 viewportPosition, int tileWidth, int tileHeight, float scale)
        {
            int i = 0;
            Vector2 destPos = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 viewPos = viewportPosition;


            int minX = (int)Math.Floor(viewportPosition.X / tileWidth);
            int minY = (int)Math.Floor(viewportPosition.Y / tileHeight);
            int maxX = (int)Math.Ceiling((rectangle.Width + viewportPosition.X) / tileWidth);
            int maxY = (int)Math.Ceiling((rectangle.Height + viewportPosition.Y) / tileHeight);

            if (minX < 0)
                minX = 0;
            if (minY < 0)
                minY = 0;
            if (maxX >= Width)
                maxX = Width - 1;
            if (maxY >= Height)
                maxY = Height - 1;

            if (viewPos.X > 0)
            {
                viewPos.X = ((int)Math.Floor(viewPos.X)) % tileWidth;
            }
            else
            {
                viewPos.X = (float)Math.Floor(viewPos.X);
            }

            if (viewPos.Y > 0)
            {
                viewPos.Y = ((int)Math.Floor(viewPos.Y)) % tileHeight;
            }
            else
            {
                viewPos.Y = (float)Math.Floor(viewPos.Y);
            }

            TileInfo info = new TileInfo();
            if (_TileInfoCache == null)
                BuildTileInfoCache(tilesets);

            // We're drawing at the center of the tile, so adjust our y offset
            destPos.Y += tileHeight / 2f;

            for (int y = minY; y <= maxY; y++)
            {
                // We're drawing at the center of the tile, so adjust the x offset
                destPos.X = rectangle.Left + tileWidth / 2f;

                for (int x = minX; x <= maxX; x++)
                {
                    i = (y * Width) + x;

                    byte flipAndRotate = FlipAndRotate[i];
                    SpriteEffects flipEffect = SpriteEffects.None;
                    float rotation = 0f;

                    if ((flipAndRotate & Layer.HorizontalFlipDrawFlag) != 0)
                    {
                        flipEffect |= SpriteEffects.FlipHorizontally;
                    }
                    if ((flipAndRotate & Layer.VerticalFlipDrawFlag) != 0)
                    {
                        flipEffect |= SpriteEffects.FlipVertically;
                    }
                    if ((flipAndRotate & Layer.DiagonallyFlipDrawFlag) != 0)
                    {
                        if ((flipAndRotate & Layer.HorizontalFlipDrawFlag) != 0 &&
                             (flipAndRotate & Layer.VerticalFlipDrawFlag) != 0)
                        {
                            rotation = (float)(Math.PI / 2);
                            flipEffect ^= SpriteEffects.FlipVertically;
                        }
                        else if ((flipAndRotate & Layer.HorizontalFlipDrawFlag) != 0)
                        {
                            rotation = (float)-(Math.PI / 2);
                            flipEffect ^= SpriteEffects.FlipVertically;
                        }
                        else if ((flipAndRotate & Layer.VerticalFlipDrawFlag) != 0)
                        {
                            rotation = (float)(Math.PI / 2);
                            flipEffect ^= SpriteEffects.FlipHorizontally;
                        }
                        else
                        {
                            rotation = -(float)(Math.PI / 2);
                            flipEffect ^= SpriteEffects.FlipHorizontally;
                        }
                    }

                    int index = Tiles[i] - 1;
                    if ((index >= 0) && (index < _TileInfoCache.Length))
                    {
                        info = _TileInfoCache[index];
                        batch.Draw(info.Texture, destPos - viewPos, info.Rectangle,
                                   Color.White * this.Opacity, rotation, new Vector2(tileWidth / 2f, tileHeight / 2f),
                                   scale, flipEffect, 0);
                    }

                    destPos.X += tileWidth;
                }

                destPos.Y += tileHeight;
            }
        }
    }

    /// <summary>
    /// A class representing a group of map Objects
    /// </summary>
    public class ObjectGroup
    {
        public SortedList<string, Object> Objects = new SortedList<string, Object>();
        public SortedList<string, string> Properties = new SortedList<string, string>();

        public string Name;
        public int Width, Height, X, Y;
        float Opacity = 1;

        /// <summary>
        /// Loads the object group from a TMX file
        /// </summary>
        /// <param name="reader">A reader to the TMX file being processed</param>
        /// <returns>An initialized ObjectGroup</returns>
        internal static ObjectGroup Load(XmlReader reader)
        {
            var result = new ObjectGroup();
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            if (reader.GetAttribute("name") != null)
                result.Name = reader.GetAttribute("name");
            if (reader.GetAttribute("width") != null)
                result.Width = int.Parse(reader.GetAttribute("width"));
            if (reader.GetAttribute("height") != null)
                result.Height = int.Parse(reader.GetAttribute("height"));
            if (reader.GetAttribute("x") != null)
                result.X = int.Parse(reader.GetAttribute("x"));
            if (reader.GetAttribute("y") != null)
                result.Y = int.Parse(reader.GetAttribute("y"));
            if (reader.GetAttribute("opacity") != null)
                result.Opacity = float.Parse(reader.GetAttribute("opacity"), NumberStyles.Any, ci);

            while (!reader.EOF)
            {
                var name = reader.Name;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "object":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        st.Read();
                                        var objects = Object.Load(st);
                                        if (!result.Objects.ContainsKey(objects.Name))
                                        {
                                            result.Objects.Add(objects.Name, objects);
                                        }
                                        else
                                        {
                                            int count = result.Objects.Keys.Count((item) => item.Equals(objects.Name));
                                            result.Objects.Add(string.Format("{0}{1}", objects.Name, count), objects);
                                        }
                                    }
                                }
                                break;
                            case "properties":
                                {
                                    using (var st = reader.ReadSubtree())
                                    {
                                        while (!st.EOF)
                                        {
                                            switch (st.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    if (st.Name == "property")
                                                    {
                                                        if (st.GetAttribute("name") != null)
                                                        {
                                                            result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                        }
                                                    }

                                                    break;
                                                case XmlNodeType.EndElement:
                                                    break;
                                            }

                                            st.Read();
                                        }
                                    }
                                }
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        public void Draw(Map result, SpriteBatch batch, Rectangle rectangle, Vector2 viewportPosition)
        {
            foreach (var objects in Objects.Values)
            {
                if (objects.Texture != null)
                {
                    objects.Draw(batch, rectangle, new Vector2(this.X * result.TileWidth, this.Y * result.TileHeight), viewportPosition, this.Opacity);
                }
            }
        }
    }

    /// <summary>
    /// A class representing a map object
    /// </summary>
    /// <remarks>
    /// A map object represents an object in a map; it has a 
    /// position, width, and height, and a collection of properties.
    /// It can be used for spawn locations, triggers, etc.
    /// In this implementation, it also has a texture 
    /// </remarks>
    public class Object
    {
        public SortedList<string, string> Properties = new SortedList<string, string>();

        public string Name, Image;
        public int Width, Height, X, Y;

        protected Texture2D _Texture;
        protected int _TexWidth, _TexHeight;

        /// <summary>
        /// The texture of the Object
        /// </summary>
        /// <remarks>
        /// This is not supplied by Tiled, and must be set manually!
        /// </remarks>
        public Texture2D Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value;
                _TexWidth = value.Width;
                _TexHeight = value.Height;
            }
        }

        /// <summary>
        /// Loads a map object from a TMX file
        /// </summary>
        /// <param name="reader">A reader to the TMX file being processed</param>
        /// <returns>An anonymous object representing the map</returns>
        internal static Object Load(XmlReader reader)
        {
            var result = new Object();

            result.Name = reader.GetAttribute("name");
            result.X = int.Parse(reader.GetAttribute("x"));
            result.Y = int.Parse(reader.GetAttribute("y"));

            /*
             * Height and width are optional on objects
             */
            int width;
            if (int.TryParse(reader.GetAttribute("width"), out width))
            {
                result.Width = width;
            }

            int height;
            if (int.TryParse(reader.GetAttribute("height"), out height))
            {
                result.Height = height;
            }

            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "properties")
                        {
                            using (var st = reader.ReadSubtree())
                            {
                                while (!st.EOF)
                                {
                                    switch (st.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            if (st.Name == "property")
                                            {
                                                if (st.GetAttribute("name") != null)
                                                {
                                                    result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                }
                                            }

                                            break;
                                        case XmlNodeType.EndElement:
                                            break;
                                    }

                                    st.Read();
                                }
                            }
                        }
                        if (reader.Name == "image")
                        {
                            result.Image = reader.GetAttribute("source");
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        /// <summary>
        /// Draws the Object
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw with</param>
        /// <param name="rectangle">The viewport (visible screen size)</param>
        /// <param name="offset">An offset to apply when rendering the viewport on-screen</param>
        /// <param name="viewportPosition">The viewport's position in the world</param>
        /// <param name="opacity">An opacity value for making the object semi-transparent (1.0=fully opaque)</param>
        public void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 offset, Vector2 viewportPosition, float opacity)
        {
            Vector2 viewPos = viewportPosition;

            int minX = (int)Math.Floor(viewportPosition.X);
            int minY = (int)Math.Floor(viewportPosition.Y);
            int maxX = (int)Math.Ceiling((rectangle.Width + viewportPosition.X));
            int maxY = (int)Math.Ceiling((rectangle.Height + viewportPosition.Y));

            if (this.X + offset.X + this.Width > minX && this.X + offset.X < maxX)
                if (this.Y + offset.Y + this.Height > minY && this.Y + offset.Y < maxY)
                {
                    int x = (int)(this.X + offset.X - viewportPosition.X);
                    int y = (int)(this.Y + offset.Y - viewportPosition.Y);
                    batch.Draw(_Texture, new Rectangle(x, y, this.Width, this.Height), new Rectangle(0, 0, _Texture.Width, _Texture.Height), Color.White * opacity);
                }
        }
    }

    /// <summary>
    /// A class representing a map created by the Tiled editor
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The Map's Tilesets
        /// </summary>
        public SortedList<string, Tileset> Tilesets = new SortedList<string, Tileset>();

        /// <summary>
        /// The Map's Layers
        /// </summary>
        public SortedList<string, Layer> Layers = new SortedList<string, Layer>();

        /// <summary>
        /// The Map's Ojbect Groups
        /// </summary>
        public SortedList<string, ObjectGroup> ObjectGroups = new SortedList<string, ObjectGroup>();

        /// <summary>
        /// The Map's properties
        /// </summary>
        public SortedList<string, string> Properties = new SortedList<string, string>();

        /// <summary>
        /// The Map's width and height
        /// </summary>
        public int Width, Height;

        /// <summary>
        /// The Map's tile width and height
        /// </summary>
        public int TileWidth, TileHeight;

        public static float Scale = 2.3f;

        /// <summary>
        /// Loads a TMX file into a Map object
        /// </summary>
        /// <param name="filename">The filename of the TMX file</param>
        /// <param name="content">The ContentManager to load textures with</param>
        /// <returns>The loaded map</returns>
        public static Map Load(string filename, ContentManager content)
        {
            var result = new Map();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            using (var stream = System.IO.File.OpenText(filename))
            using (var reader = XmlReader.Create(stream, settings))
                while (reader.Read())
                {
                    var name = reader.Name;

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.DocumentType:
                            if (name != "map")
                                throw new Exception("Invalid map format");
                            break;
                        case XmlNodeType.Element:
                            switch (name)
                            {
                                case "map":
                                    {
                                        result.Width = int.Parse(reader.GetAttribute("width"));
                                        result.Height = int.Parse(reader.GetAttribute("height"));
                                        result.TileWidth = (int)(int.Parse(reader.GetAttribute("tilewidth")) * Scale);
                                        result.TileHeight = (int)(int.Parse(reader.GetAttribute("tileheight")) * Scale);
                                    }
                                    break;
                                case "tileset":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var tileset = Tileset.Load(st);
                                            result.Tilesets.Add(tileset.Name, tileset);
                                        }
                                    }
                                    break;
                                case "layer":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var layer = Layer.Load(st);
                                            if (null != layer)
                                            {
                                                result.Layers.Add(layer.Name, layer);
                                            }
                                        }
                                    }
                                    break;
                                case "objectgroup":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            st.Read();
                                            var objectgroup = ObjectGroup.Load(st);
                                            result.ObjectGroups.Add(objectgroup.Name, objectgroup);
                                        }
                                    }
                                    break;
                                case "properties":
                                    {
                                        using (var st = reader.ReadSubtree())
                                        {
                                            while (!st.EOF)
                                            {
                                                switch (st.NodeType)
                                                {
                                                    case XmlNodeType.Element:
                                                        if (st.Name == "property")
                                                        {
                                                            if (st.GetAttribute("name") != null)
                                                            {
                                                                result.Properties.Add(st.GetAttribute("name"), st.GetAttribute("value"));
                                                            }
                                                        }

                                                        break;
                                                    case XmlNodeType.EndElement:
                                                        break;
                                                }

                                                st.Read();
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.Whitespace:
                            break;
                    }
                }

            foreach (var tileset in result.Tilesets.Values)
            {
                tileset.Texture = content.Load<Texture2D>(
                    Path.Combine(Path.GetDirectoryName(tileset.Image), Path.GetFileNameWithoutExtension(tileset.Image))
                );
            }

            foreach (var objects in result.ObjectGroups.Values)
            {
                foreach (var item in objects.Objects.Values)
                {
                    if (item.Image != null)
                    {
                        item.Texture = content.Load<Texture2D>
                        (
                            Path.Combine
                            (
                                Path.GetDirectoryName(item.Image),
                                Path.GetFileNameWithoutExtension(item.Image)
                            )
                        );
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Draws the Map
        /// </summary>
        /// <param name="batch">The SpriteBatch to draw with</param>
        /// <param name="rectangle">The Viewport to draw within</param>
        /// <param name="viewportPosition">The position of the viewport within the map</param>
        public void Draw(SpriteBatch batch, Rectangle rectangle, Vector2 viewportPosition)
        {
            // Draw the layers
            foreach (Layer layers in Layers.Values)
            {
                layers.Draw(batch, Tilesets.Values, rectangle, viewportPosition, TileWidth, TileHeight, Scale);
            }

            // Draw the objects
            foreach (var objectgroups in ObjectGroups.Values)
            {
                objectgroups.Draw(this, batch, rectangle, viewportPosition);
            }
        }


    }
}

