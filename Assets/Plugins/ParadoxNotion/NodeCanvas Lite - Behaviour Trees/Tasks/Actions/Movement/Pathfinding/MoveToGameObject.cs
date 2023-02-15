using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace NodeCanvas.Tasks.Actions
{

    [Name("Seek (GameObject)")]
    [Category("Movement/Pathfinding")]
    public class MoveToGameObject : ActionTask<NavMeshAgent>
    {

        [RequiredField]
        public BBParameter<GameObject> target;
        public BBParameter<float> speed = 4;
        public BBParameter<float> keepDistance = 0.1f;

        private Vector3? lastRequest;

        protected override string info {
            get { return "Seek " + target; }
        }

        protected override void OnExecute() {
            if ( target.value == null ) { EndAction(false); return; }
            agent.speed = speed.value;
            if ( Vector3.Distance(agent.transform.position, target.value.transform.position) <= agent.stoppingDistance + keepDistance.value ) {
                EndAction(true);
                return;
            }
        }

        protected override void OnUpdate() {
            if ( target.value == null ) { EndAction(false); return; }
            var pos = target.value.transform.position;
            if ( lastRequest != pos ) {
                if ( !agent.SetDestination(pos) ) {
                    EndAction(false);
                    return;
                }
            }

            // Currently ensure it will get path
            agent.SetDestination(pos);
            lastRequest = pos;

            if ( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value ) {
                //Debug.Log($"remain dist: {agent.remainingDistance}\n stop dist: {agent.stoppingDistance} keep dist: {keepDistance.value} = {agent.stoppingDistance + keepDistance.value}");
                EndAction(true);
            }
        }

        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
            lastRequest = null;
        }

        public override void OnDrawGizmosSelected() {
            if ( target.value != null ) {
                Gizmos.DrawWireSphere(target.value.transform.position, keepDistance.value);
            }
        }
    }
}