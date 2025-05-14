using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    Zombie_TopLayer topLayer;
    void EnterTiredMotion() { }
    void ExitTiredMotion() { }

    //플레이어와의 거리를 반환하는 get property
    float playerDist => 1.0f;
    void Awake()
    {
        //스테이트머신 생성
        topLayer = new Zombie_TopLayer(this, new ZombieFSMVals());

        //스테이트머신 초기화
        topLayer.OnStateEnter();
    }
    private void Update()
    {
        //스테이트머신 업데이트
        topLayer.OnStateUpdate();
    }
    public class ZombieFSMVals : FSMVals
    {
        public float stamina = 100.0f;
    }
    public class Zombie_TopLayer : TopLayer<Zombie, ZombieFSMVals>
    {
        //origin = 이 스테이트머신을 사용하는 애
        //values = 공용 변수 부분, 여기선 ZombieFSMVals의 인스턴스
        public Zombie_TopLayer(Zombie origin, ZombieFSMVals values) : base(origin, values)
        {
            //레이어의 기본 스테이트를 Zombie_Walking으로 설정 (상위 스테이트를 자기 자신으로 설정)
            defaultState = new Zombie_Walking(origin, this);

            //기본 스테이트를 "Walking" 이름으로 저장
            AddState("Walking", defaultState);

            //Zombie_Detected 스테이트를 "Detected" 이름으로 저장
            AddState("Detected", new Zombie_Detected(origin, this));
        }
        //플레이어를 발견하지 않고 걸어다니고 있는 상태
        public class Zombie_Walking : State<Zombie, ZombieFSMVals>
        {
            public Zombie_Walking(Zombie origin, Layer<Zombie, ZombieFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                //좀비가 걷는다

                //걷는 도중 스태미나 재생
                values.stamina += 1.0f;

                //거리가 일정 이하로 떨어지면...
                if(origin.playerDist < 10.0f)
                {
                    //"Detected" 상태로 전환
                    parentLayer.ChangeState("Detected");
                }
            }
        }
        //플레이어를 발견한 상태
        public class Zombie_Detected : Layer<Zombie, ZombieFSMVals>
        {
            public Zombie_Detected(Zombie origin, Layer<Zombie, ZombieFSMVals> parent) : base(origin, parent)
            {
                //기본 스테이트를 Zombie_Running 으로 설정
                defaultState = new Zombie_Running(origin, this);

                //기본 스테이트를 "Running" 이름으로 추가
                AddState("Running", defaultState);

                //Zombie_Tired 스테이트를 "Tired" 이름으로 추가
                AddState("Tired", new Zombie_Tired(origin, this));
            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                //플레이어 감지 시 해야 할 것들
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                //거리가 일정 이상으로 벌어지면..
                if(origin.playerDist > 15.0f)
                {
                    //"Walking" 스테이트로 전환
                    parentLayer.ChangeState("Walking");
                }
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                //플레이어 감지 종료 시 해야 할 것들
            }
            //감지 도중 달리는 상태
            public class Zombie_Running : State<Zombie, ZombieFSMVals>
            {
                public Zombie_Running(Zombie origin, Layer<Zombie, ZombieFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    //좀비가 플레이어를 향해 달린다

                    //스태미나 감소
                    values.stamina -= 1.0f;

                    //스태미나가 다 떨어지면...
                    if (values.stamina <= 0.0f)
                    {
                        //"Tired" 상태로 전환
                        parentLayer.ChangeState("Tired");
                    }
                }
            }
            //감지 도중 지친 상태
            public class Zombie_Tired : State<Zombie, ZombieFSMVals>
            {
                public Zombie_Tired(Zombie origin, Layer<Zombie, ZombieFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    //지친 모션 플레이
                    origin.EnterTiredMotion();
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    //지쳐 있는 도중 스태미나 재생
                    values.stamina += 1.0f;

                    //스태미나가 일정 이상 다시 차면...
                    if (values.stamina >= 50.0f)
                    {
                        //"Running" 상태로 전환
                        parentLayer.ChangeState("Running");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    //지친 모션 중단
                    origin.ExitTiredMotion();
                }
            }
        }
    }
}
