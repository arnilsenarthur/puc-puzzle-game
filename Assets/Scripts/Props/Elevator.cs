using UnityEngine;
using System.Linq;
using Game.Data;
using Game.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Props
{
    [System.Serializable]
    public class ElevatorLevel
    {
        public Vector3 position;
        public BoolValue isOnLevel;
        public Door[] doors;
    }

    public class Elevator : MonoBehaviour
    {
        #region References
        public Rigidbody elevatorGround;
        public Door[] elevatorDoors;
        public ElevatorLevel[] levels;
        public int initInLevel = 0;
        public Value<string> levelIndicator;
        #endregion

        #region State
        public IntValue currentLevel;
        #endregion

        #region Settings
        public float speed = 2f;
        #endregion

        #region Private Fields
        private int _currentLevel = 0;
        private int _targetLevel = -1;
        #endregion

        private void Start()
        {
            if(levels.Length > 0)       
                SetCurrentLevel(initInLevel);

            if(elevatorGround == null)
                elevatorGround = GetComponent<Rigidbody>();
        }

        public void SetCurrentLevel(int level)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                ElevatorLevel l = levels[i];
                bool isOnLevel = i == level;
                l.isOnLevel.value = isOnLevel;
                
                foreach(Door door in l.doors)
                    door.open.value = isOnLevel;
            }

            if (level != -1)
            {
                transform.position = levels[level].position;
                currentLevel.value = level + 1;
                
                levelIndicator.value = currentLevel.value.ToString();
            }

            foreach(Door door in elevatorDoors)
            {
                door.open.value = level != -1;
            }

            _currentLevel = level;

        }

        public void GoToLevel(int level)
        {
            if (level == _currentLevel || _targetLevel != -1)
                return;

            SetCurrentLevel(-1);
            _targetLevel = level;
        }

        private void FixedUpdate()
        {
            if (_currentLevel != _targetLevel && _targetLevel != -1)
            {
                //check if one door is not rally open
                bool allClosed = elevatorDoors.All(door => door.IsReallyClosed);
                
                if (allClosed)
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, levels[_targetLevel].position, speed * Time.fixedDeltaTime);
         
                    elevatorGround.MovePosition(pos);

                    if(elevatorGround.gameObject != gameObject)
                        transform.position = pos;

                    float ds = float.MaxValue;
                    int level = 0;

                    for (int i = 0; i < levels.Length; i++)
                    {
                        float distance = Vector3.Distance(transform.position, levels[i].position);
                        if (distance < ds)
                        {
                            ds = distance;
                            level = i;
                        }
                    }

                    currentLevel.value = level + 1;
                    //currentLevel.value.ToString();
                    levelIndicator.value = ds < 1f ? (level + 1).ToString() : ">>";
                }

                if (transform.position == levels[_targetLevel].position)
                {
                    SetCurrentLevel(_targetLevel);
                    _targetLevel = -1;
                }
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Elevator))]
    public class ElevatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Elevator elevator = (Elevator)target;

            for(int i = 0; i < elevator.levels.Length; i ++)
                if (GUILayout.Button("Go to level " + (i + 1)))       
                    elevator.GoToLevel(i);
        }
    }
    #endif
}