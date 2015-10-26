﻿using UnityEngine;
using System.Collections.Generic;
using GalaxyShared;

public class ClientSector  {

    public List<GameObject> gameObjects;
    public GalaxyCoord pos;

    public ClientSector(GalaxyCoord pos, List<GameObject> gameObjects)
    {
        this.pos = pos;
        this.gameObjects = gameObjects;
    }
	
    public void Dispose(Stack<GameObject> starPool)
    {
        foreach (GameObject go in gameObjects)
        {
            starPool.Push(go);
        }
    }
}
