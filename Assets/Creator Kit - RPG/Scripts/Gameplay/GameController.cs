using UnityEngine;
using RPGM.Core;
using RPGM.Gameplay;

namespace RPGM.Gameplay
{
    /// <summary>
    /// The global game controller. It contains the game model and executes the schedule.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        //This model is public and can be modified in the inspector.
        //The reference is shared where needed, and Unity will deserialize
        //over the shared reference, rather than create a new instance.
        //To preserve this behaviour, this script must be deserialized last.
        public GameModel model;

        protected virtual void OnEnable()
        {
            Schedule.SetModel<GameModel>(model);
        }

        protected virtual void Update()
        {
            Schedule.Tick();
        }
    }
}