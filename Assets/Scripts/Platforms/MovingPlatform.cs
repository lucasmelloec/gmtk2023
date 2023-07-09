using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    float speed;
    List<Vector3> waypoints;
    List<float> waypointPositions;
    bool started = false;
    DateTime platformStart;
    bool playerOnTop = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!started)
        {
            started = true;
            transform.position = waypoints[0];
        }

        platformStart = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        var elapsedTime = (DateTime.Now - platformStart).TotalSeconds;
        var currentLinearPos = elapsedTime * speed;
        var maxLinearPos = waypointPositions.Last();

        var completeRounds = (int)(currentLinearPos / maxLinearPos);
        currentLinearPos = currentLinearPos - completeRounds * maxLinearPos;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var wp0LinearPos = waypointPositions[i];
            var wp1LinearPos = waypointPositions[i+1];
            var wp0 = waypoints[i];
            var wp1 = waypoints[i + 1];
            if (wp0LinearPos <= currentLinearPos && currentLinearPos <= wp1LinearPos)
            {
                var percentage = (currentLinearPos - wp0LinearPos) / (wp1LinearPos - wp0LinearPos);
                var pos = wp0 + (wp1 - wp0) * (float)percentage;
                var delta = pos - transform.position;
                transform.position = pos;

                if (playerOnTop)
                {
                    Player.singleton.transform.position += delta;
                }
            }
        }
    }

    public void InitializeParams(List<Vector3> waypoints, float speed)
    {
        this.waypoints = waypoints;
        this.speed = speed;

        if ((waypoints.Last() - waypoints.First()).magnitude > 0.01f)
        {
            waypoints.Add(waypoints[0]);
        }

        waypointPositions = new List<float>();
        var currentLength = 0f;
        var lastPoint = waypoints[0];
        for (int i = 0; i < waypoints.Count; i++)
        {
            var wp = waypoints[i];
            currentLength = currentLength + (wp - lastPoint).magnitude;
            waypointPositions.Add(currentLength);
            lastPoint = wp;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.singleton.gameObject)
        {
            playerOnTop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == Player.singleton.gameObject)
        {
            playerOnTop = false;
        }
    }
}
