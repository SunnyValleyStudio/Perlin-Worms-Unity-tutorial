using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerlinWorm
{
    private Vector2 currentDirection;
    private Vector2 currentPosition;
    private Vector2 convergancePoint;
    NoiseSettings noiseSettings;
    public bool moveToConvergancepoint = false;
    [Range(0.5f, 0.9f)]
    public float weight = 0.6f;

    public PerlinWorm(NoiseSettings noiseSettings, Vector2 startPosition, Vector2 convergancePoint)
    {
        currentDirection = Random.insideUnitCircle.normalized;
        this.noiseSettings = noiseSettings;
        this.currentPosition = startPosition;
        this.convergancePoint = convergancePoint;
        this.moveToConvergancepoint = true;
    }

    public PerlinWorm(NoiseSettings noiseSettings, Vector2 startPosition)
    {
        currentDirection = Random.insideUnitCircle.normalized;
        this.noiseSettings = noiseSettings;
        this.currentPosition = startPosition;
        this.moveToConvergancepoint = false;
    }

    public Vector2 MoveTowardsConvergancePoint()
    {
        Vector3 direction = GetPerlinNoiseDirection();
        var directionToConvergancePoint = (this.convergancePoint - currentPosition).normalized;
        var endDirection = ((Vector2)direction * (1 - weight) + directionToConvergancePoint * weight).normalized;
        currentPosition += endDirection;
        return currentPosition;
    }

    public Vector2 Move()
    {
        Vector3 direction = GetPerlinNoiseDirection();
        currentPosition += (Vector2)direction;
        return currentPosition;
    }

    private Vector3 GetPerlinNoiseDirection()
    {
        float noise = NoiseHelper.SumNoise(currentPosition.x, currentPosition.y, noiseSettings); //0-1
        float degrees = NoiseHelper.RangeMap(noise, 0, 1, -90, 90);
        currentDirection = (Quaternion.AngleAxis(degrees, Vector3.forward) * currentDirection).normalized;
        return currentDirection;
    }

    public List<Vector2> MoveLength(int length)
    {
        var list = new List<Vector2>();
        foreach (var item in Enumerable.Range(0, length))
        {
            if (moveToConvergancepoint)
            {
                var result = MoveTowardsConvergancePoint();
                list.Add(result);
                if (Vector2.Distance(this.convergancePoint, result) < 1)
                {
                    break;
                }
            }
            else
            {
                var result = Move();
                list.Add(result);
            }


        }
        if (moveToConvergancepoint)
        {
            while (Vector2.Distance(this.convergancePoint, currentPosition) > 1)
            {
                weight = 0.9f;
                var result = MoveTowardsConvergancePoint();
                list.Add(result);
                if (Vector2.Distance(this.convergancePoint, result) < 1)
                {
                    break;
                }
            }
        }

        return list;
    }
}
