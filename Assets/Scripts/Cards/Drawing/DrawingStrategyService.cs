using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingStrategyService
{
    private readonly Dictionary<Type, IDrawingStrategy> drawingStrategies = new Dictionary<Type, IDrawingStrategy>();

    public DrawingStrategyService()
    {
        drawingStrategies.Add(typeof(RandomDrawingStrategy), new RandomDrawingStrategy());
        drawingStrategies.Add(typeof(TopDeckStrategy), new TopDeckStrategy());
        drawingStrategies.Add(typeof(BottomDeckStrategy), new BottomDeckStrategy());
        drawingStrategies.Add(typeof(MiddleDrawStrategy), new MiddleDrawStrategy());
    }

    public T GetStrategy<T>() where T : IDrawingStrategy
    {
        return (T)drawingStrategies[typeof(T)];
    }
}
