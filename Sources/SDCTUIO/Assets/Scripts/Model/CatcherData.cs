using UnityEngine;

public class CatcherData : MonoBehaviour
{
    public DebrisData Target{get;private set;}
    public double MinutesBeforeCatch{get;set;}//in minutes
    public Vector3 LastPos=new Vector3(0,0,0);
    
    public double GetSpeed(){
        Vector3 posDiff=(transform.position-LastPos)*SimulationManager.Instance.SimulationScaleFactor;
        double distance=Math.Sqrt(posDiff.x*posDiff.x+posDiff.y*posDiff.y+posDiff.z*posDiff.z);

        return distance/Time.deltaTime;
        }

    public double GetTargetDistance(){
        var speed=GetSpeed();
        return speed*MinutesBeforeCatch*60;
        }

    public Catcher(double MinutesBeforeCatch,Debris Target){
        this.MinutesBeforeCatch=MinutesBeforeCatch;
        this.Target=Target;

    }
    public string GetTargetName(){
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
