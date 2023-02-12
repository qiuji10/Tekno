using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using System;
using UnityEditor.PackageManager.Requests;

namespace NodeCanvas.Tasks.Actions{

	[Category("Movement/Pathfinding")]
	[Description("Wander within a polygon area constructed by gameobjects")]
	public class RandomPositionInPolygon : ActionTask<UnityEngine.AI.NavMeshAgent>{

        public BBParameter<List<Transform>> polygonArea;
        public BBParameter<float> speed;
        private Vector3? lastRequest;

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

			if (polygonArea.value.Count < 3)
				return;


			
			EndAction(true);
		}

		//Called once per frame while the action is active.
		protected override void OnUpdate(){

            agent.speed = speed.value;

            
        }

		//Called when the task is disabled.
		protected override void OnStop(){
            if (lastRequest != null && agent.gameObject.activeSelf)
            {
				agent.ResetPath();
            }
            lastRequest = null;
        }

		//Called when the task is paused.
		protected override void OnPause(){ OnStop(); }

		public override void OnDrawGizmosSelected()
		{
			if (polygonArea != null)
			{
                if (GeometryUtils.PointInPolygon(agent.transform.position, polygonArea.value))
                {
                    PolygonUtility.DebugIsInPolygon(polygonArea.value, Color.green);
                }
                else
                {
                    PolygonUtility.DebugIsInPolygon(polygonArea.value, Color.red);
                }
            }
		}
    }
}