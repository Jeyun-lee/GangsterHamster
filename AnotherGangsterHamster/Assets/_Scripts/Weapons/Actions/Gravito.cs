using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons.Checkpoint;
using Matters.Gravity;

namespace Weapons.Actions
{
    public class Gravito : WeaponAction
    {
        enum GravityDir
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            FORWARD,
            BACK
        }

        public float gravityChangeTime;
        public float penetratePadding;

        private Dictionary<GravityDir, Vector3> _gravityDirDict = new Dictionary<GravityDir, Vector3>();
        private GravitoStatus _currentGravitoStatus = GravitoStatus.Idle; 
        private CheckpointManager _checkpoint;
        private RaycastHit _aTypeHit;
        private Vector3 _currentChangeGravityDir;

        private CheckpointManager Checkpoint
        {
            get
            {
                if(_checkpoint == null)
                {
                    _checkpoint = FindObjectOfType<CheckpointManager>();
                }

                return _checkpoint;
            }
        }

        private float _currentGravityChangeTime = 0f;  
        private bool isChangedGravity = false;
        private bool isReseting = false; 

        private new void Awake()
        {
            base.Awake();

            _weaponEnum = WeaponEnum.Gravito;

            _gravityDirDict.Add(GravityDir.UP, Vector3.up);
            _gravityDirDict.Add(GravityDir.DOWN, Vector3.down);
            _gravityDirDict.Add(GravityDir.LEFT, Vector3.left);
            _gravityDirDict.Add(GravityDir.RIGHT, Vector3.right);
            _gravityDirDict.Add(GravityDir.FORWARD, Vector3.forward);
            _gravityDirDict.Add(GravityDir.BACK, Vector3.back);
        }

        public override void FireWeapon()
        {
            if (_currentGravitoStatus == GravitoStatus.Idle &&
               !isReseting)
            {
                if(Physics.Raycast(MainCameraTransform.position, MainCameraTransform.forward, out RaycastHit hit) && hit.transform.CompareTag("ATYPEOBJECT"))
                {
                    _myRigid.constraints = RigidbodyConstraints.FreezeAll;
                    _fireDir = MainCameraTransform.forward;
                    _currentGravitoStatus = GravitoStatus.Stickly;
                    transform.position = hit.point - (_fireDir * (transform.localScale.y + penetratePadding));
                    transform.rotation = Quaternion.LookRotation(_fireDir) * Quaternion.Euler(90, 0, 0);

                    _aTypeHit = hit;
                    _currentChangeGravityDir = CheckDir(hit.normal);

                    // 일정 각도 차 만큼 돌리고 픔
                    // 차 값, 위치 이동 값
                    // 쿼터니언 끼리의 차를 구하고
                    // 그걸 계속해서 저장한 뒤
                    // 그게 변환되면
                    // 새로이 변환된 값을 이용해서 각도와 위치를 돌린다.



                    // 위치는 어떤 기준이 되는 방향 벡터를 기준으로
                    // 처음 각도를 할당한 뒤
                    // 나중에 들어온 값과 비교해서 그거의 차 만큼
                    // 얘를 돌려주면 됨
                }
            }
        }

        public override void UseWeapon()
        {
            if(_currentGravitoStatus == GravitoStatus.Stickly && !isChangedGravity)
            {
                if (_currentChangeGravityDir == Vector3.up) return;

                _currentGravitoStatus = GravitoStatus.ChangeGravity;
                _currentGravityChangeTime = 0f;
                isChangedGravity = true;

                Checkpoint.SetStartCheckpoint(PlayerBaseTransform.forward);
                Checkpoint.SetEndCheckpoint(_currentChangeGravityDir);

                GravityManager.ChangeGlobalGravityDirection(-_currentChangeGravityDir);
            }
        }

        public override void ResetWeapon()
        {
            if (isReseting)
                return;

            _myRigid.constraints = RigidbodyConstraints.None;
            
            if (!isChangedGravity)
            {
                _currentGravitoStatus = GravitoStatus.Idle;
                transform.rotation = Quaternion.identity;
                Update();
                return;
            }

            _currentGravitoStatus = GravitoStatus.Reset;
            _currentGravityChangeTime = 0f;
            isChangedGravity = false;
            isReseting = true;

            Checkpoint.startCheckpoint.rotation = PlayerBaseTransform.rotation;
            Checkpoint.endCheckpoint.rotation = Quaternion.Euler(new Vector3(0, PlayerBaseTransform.rotation.eulerAngles.y, 0));
            
            GravityManager.ChangeGlobalGravityDirection(Vector3.down);

            Update();
        }

        public override bool IsHandleWeapon()
        {
            return _currentGravitoStatus == GravitoStatus.Idle;
        }

        private void Update()
        {
            switch(_currentGravitoStatus)
            {
                case GravitoStatus.Idle:
                    Vector3 gravitoHandPos = HandPosition - (PlayerBaseTransform.right / 4);

                    if(Vector3.Distance(transform.position, gravitoHandPos) > 1f)
                    {
                        transform.position = gravitoHandPos;
                    }
                    _myRigid.velocity = (gravitoHandPos - transform.position) * 20;
                    _myRigid.angularVelocity = _myRigid.angularVelocity / 2;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.5f);
                    break;
                case GravitoStatus.Stickly:

                    break;
                case GravitoStatus.ChangeGravity:
                    _currentGravityChangeTime += Time.deltaTime / gravityChangeTime;

                    if (_currentGravityChangeTime >= 1f)
                    {
                        PlayerBaseTransform.rotation = Checkpoint.endCheckpoint.rotation;
                        _currentGravitoStatus = GravitoStatus.Stickly;
                    }
                    else
                    {
                        PlayerBaseTransform.rotation = Quaternion.Lerp(
                            Checkpoint.startCheckpoint.rotation,
                            Checkpoint.endCheckpoint.rotation,
                            _currentGravityChangeTime);
                    }
                    break;
                case GravitoStatus.Reset: 
                    _currentGravityChangeTime += Time.deltaTime / gravityChangeTime;

                    if(_currentGravityChangeTime >= 1f)
                    {
                        PlayerBaseTransform.rotation = Checkpoint.endCheckpoint.rotation;
                        _currentGravitoStatus = GravitoStatus.Idle;
                        transform.rotation = Quaternion.identity;
                        isReseting = false;
                    }
                    else
                    {
                        PlayerBaseTransform.rotation = Quaternion.Lerp(
                            Checkpoint.startCheckpoint.rotation,
                            Checkpoint.endCheckpoint.rotation,
                            _currentGravityChangeTime);
                    }
                    break;
            }
        }

        private Vector3 CheckDir(Vector3 dir)
        {
            GravityDir gravityDir = GravityDir.UP;

            for(int i = 0; i < 6; i++)
            {
                if(Vector3.Angle(_gravityDirDict[(GravityDir)i], dir) <= 45)
                {
                    gravityDir = (GravityDir)i;
                    break;
                }
            }

            return _gravityDirDict[gravityDir];
        }
    }
}