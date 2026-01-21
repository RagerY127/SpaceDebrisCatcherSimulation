using UnityEngine;
using System;
public class CatcherData : MonoBehaviour
{
    public DebrisData Target{get;private set;}
    public double MinutesBeforeCatch{get;set;}//in minutes
    public Vector3 LastPos=new Vector3(0,0,0);
    
    public double GetSpeed(){
        Vector3 posDiff=(transform.position-LastPos)*SimulationManager.ScaleFactor;
        double distance=Math.Sqrt(posDiff.x*posDiff.x+posDiff.y*posDiff.y+posDiff.z*posDiff.z);

        return distance/Time.deltaTime;
        }

    public double GetTargetDistance(){
        var speed=GetSpeed();
        return speed*MinutesBeforeCatch*60;
        }

    public CatcherData(double MinutesBeforeCatch,DebrisData Target){
        this.MinutesBeforeCatch=MinutesBeforeCatch;
        this.Target=Target;

    }
    public string GetTargetName(){
        return "";
        //return Target.GetInfos["name"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LastPos=transform.position;
    }
}