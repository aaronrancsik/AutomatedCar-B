﻿namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing.Printing;
    using System.IO;
    using System.Linq;
    using Avalonia;
    using Avalonia.Controls.Shapes;
    using Avalonia.Media;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ReactiveUI;

    public class World : ReactiveObject
    {
        // private static readonly System.Lazy<World> lazySingleton = new System.Lazy<World> (() => new World());
        // public static World Instance { get { return lazySingleton.Value; } }

        private AutomatedCar _controlledCar;

        public static World Instance { get; } = new World();

        public ObservableCollection<WorldObject> WorldObjects { get; } = new ObservableCollection<WorldObject>();

        public AutomatedCar ControlledCar
        {
            get => this._controlledCar;
            set => this.RaiseAndSetIfChanged(ref this._controlledCar, value);
        }

        private List<WorldObject> trees;
        private List<Sign> signs;
        private List<WorldObject> roads;
        private List<WorldObject> npcs; // for 2nd sprint

        public int Width { get; set; }

        public int Height { get; set; }

        public List<WorldObject> NPCs { get; set; }

        private World()
        {

        }

        public void AddObject(WorldObject worldObject)
        {
            this.WorldObjects.Add(worldObject);
        }

        private static List<WorldObject> ReadWorldObjects(string filename)
        {
            List<WorldObject> allObjects;

            JObject worldObjectsInFile = JObject.Parse(File.ReadAllText("..\\..\\..\\Assets\\" + filename));

            IList<JToken> results = worldObjectsInFile["objects"].Children().ToList();

            allObjects = results.Select(r => new WorldObject
            {
                X = (int)r["x"],
                Y = (int)r["y"],
                FileName = (string)r["type"],
                Angle = Math.Acos((double)r["m11"]),
            }).ToList();

            return allObjects;
        }

        private static List<WorldObject> ReadPolygons(string filename)
        {
            List<WorldObject> polygons;

            JObject polygonsInFile = JObject.Parse(File.ReadAllText("..\\..\\..\\Assets\\" + filename));

            IList<JToken> polygonsInJson = polygonsInFile["objects"].Children().ToList();

            polygons = polygonsInJson.Select(polygon => new WorldObject
            {
                FileName = (string)polygon["typename"],
                Polygon = polygon["polys"].Children()["points"].Select(pointlist => new Polygon
                {
                    Points = pointlist.Select(point => new Point((double)point[0], (double)point[1])).ToList(),
                }).ToList().ToArray(),
            }).ToList();

            return polygons;
        }

        private static List<WorldObject> ReadRotationPoints(string filename)
        {
            List<WorldObject> rotationPoints;

            var rotationPointsInFile = JArray.Parse(File.ReadAllText("..\\..\\..\\Assets\\" + filename));

            rotationPoints = rotationPointsInFile.Children().ToList().Select(rotationpoint => new WorldObject
            {
                FileName = rotationpoint["name"].ToString().Substring(0, rotationpoint["name"].ToString().LastIndexOf(".")),
                RotationCenterPointX = (int)rotationpoint["x"],
                RotationCenterPointY = (int)rotationpoint["y"],
            }).ToList();

            return rotationPoints;
        }

        public static World GetInstance()
        {
            JObject configFilenames = JObject.Parse(File.ReadAllText("..\\..\\..\\Assets\\config.json"));

            var allObjects = ReadWorldObjects(configFilenames["world_objects"].ToString());
            var onlyPolygons = ReadPolygons(configFilenames["polygons"].ToString());
            var onlyRotationPoints = ReadRotationPoints(configFilenames["rotation_points"].ToString());

            // got bored with LINQ, could be refactored in necessary
            foreach (var worldObject in allObjects)
            {
                foreach (var polygon in onlyPolygons)
                {
                    if (worldObject.FileName == polygon.FileName) worldObject.Polygon = polygon.Polygon;
                }

                foreach (var rotationPoint in onlyRotationPoints)
                {
                    if (worldObject.FileName == rotationPoint.FileName)
                    {
                        worldObject.RotationCenterPointX = rotationPoint.RotationCenterPointX;
                        worldObject.RotationCenterPointY = rotationPoint.RotationCenterPointY;
                    }
                }
            }

            Instance.roads = allObjects.Where(r => r.FileName.Contains("road_")).ToList();
            Instance.trees = allObjects.Where(r => r.FileName.Contains("tree")).ToList();
            Instance.signs = allObjects.Where(r => r.FileName.Contains("roadsign_")).Select(s => new Sign
            {
                X = s.X,
                Y = s.Y,
                FileName = s.FileName,
                Angle = s.Angle,
                Text = s.FileName.Substring(s.FileName.LastIndexOf("_") + 1),
                RotationCenterPointX = s.RotationCenterPointX,
                RotationCenterPointY = s.RotationCenterPointY,
                Polygon = s.Polygon,
            }).ToList();

            return Instance; // is this required? the method could be void
        }

        /// <summary>
        /// Getting NPCs in given area.
        /// </summary>
        /// <param name="a">Point A of defined area</param>
        /// <param name="b">Point B of defined area</param>
        /// <param name="c">Point C of defined area</param>
        /// <returns>List of world objects containing all NPCs in given area.</returns>
        public List<WorldObject> getNPCsInTriangle(int a, int b, int c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Getting Roads in given area.
        /// </summary>
        /// <param name="a">Point A of defined area</param>
        /// <param name="b">Point B of defined area</param>
        /// <param name="c">Point C of defined area</param>
        /// <returns>List of world objects containing all Roads in given area.</returns>
        public List<WorldObject> getRoadsInTriangle(int a, int b, int c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Getting Trees in given area.
        /// </summary>
        /// <param name="a">Point A of defined area</param>
        /// <param name="b">Point B of defined area</param>
        /// <param name="c">Point C of defined area</param>
        /// <returns>List of world objects containing all Trees in given area.</returns>
        public List<WorldObject> getTreesInTriangle(int a, int b, int c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Getting Signs in given area.
        /// </summary>
        /// <param name="a">Point A of defined area</param>
        /// <param name="b">Point B of defined area</param>
        /// <param name="c">Point C of defined area</param>
        /// <returns>List of world objects containing all Signs in given area.</returns>
        public List<WorldObject> getSignsInTriangle(int a, int b, int c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Getting WorldObjects in given rectangle area.
        /// </summary>
        /// <param name="a">Point A of defined area</param>
        /// <param name="b">Point B of defined area</param>
        /// <param name="c">Point C of defined area</param>
        /// <param name="d">Point D of defined area</param>
        /// <returns>List of world objects containing all WorldObjects in given area.</returns>
        public List<WorldObject> getWorldObjectsInRectangle(int a, int b, int c, int d)
        {
            throw new NotImplementedException();
        }
    }
}