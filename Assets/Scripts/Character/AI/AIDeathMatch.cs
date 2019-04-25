using System.Collections;
using UnityEngine;

namespace AI
{
    class AIDeathMatch : AIBehaviourState
    {
        Transform target;
        Transform characterTransform;
        CharacterMovementAI characterMovementAI;
        CharacterMovement enemyCharacter;
        Vector3 position;
        float deltaTime;
        float lastDistance;
        float timer;
        float maxAttackTimer = 1;
        float maxBlockTimer = 1;
        float maxBlockingTime = 4;
        float blockTimer = 0;
        float attackTimer = 0;
        float blockProbability = 0.025f;
        float maxWaitTimer = 3f;
        float waitTimer = 0;

        Vector3 prevTargetPosition;
        Vector3 targetPosition;

        public AIDeathMatch(AIState state, CharacterMovementAI character) : base(state, character)
        {
            destination = character.transform.position;
            Initialize();
            this.characterMovementAI = character;
            characterTransform = character.transform;
            Character.Dead += CheckDeadTarget;
        }

        private void CheckDeadTarget(Character character)
        {
            if (!enemyCharacter) return;
            if (character == enemyCharacter.character)
                ResetTarget();
        }

        public override void ClearForGC()
        {
            Character.Dead -= CheckDeadTarget;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override bool Execute()
        {
            position = characterTransform.position;
            deltaTime = Time.deltaTime;
            timer += deltaTime;
            if (target == null)
            {
                target = GetNearest(out enemyCharacter);
            }
            else
            {
                targetPosition = target.position;
                lastDistance = DistanceToTarget();
                if (lastDistance < 3)
                {
                    attackTimer -= deltaTime;
                    blockTimer -= deltaTime;
                    if(attackTimer <=0)
                    {
                        ResetTimers();
                        HandleAttack();
                    }
                    if(enemyCharacter && enemyCharacter.isAttacking && Engine.Math.Probability(blockProbability))
                    {
                        ResetTimers();
                        //ShieldUp();
                    }
                    if (lastDistance < 1.5f)
                        characterTransform.position += -Engine.Vector.Direction(position, targetPosition) * deltaTime;
                }
                else
                {              
                    if (waitTimer <= 0)
                    {
                        ResetTimers();
                        HandlePath();
                    }
                    else
                        waitTimer -= deltaTime;
                }
            }
            return true;
        }

        void ResetTimers(bool attack = false)
        {
            waitTimer = Random.Range(0,maxWaitTimer);
            SetAttackTimer();
            SetBlockTimer();
            if (attack)
                attackTimer = blockTimer - 0.1f;
        }

        float DistanceToTarget()
        {
            return Vector3.Distance(targetPosition, position);
        }

        void SetAttackTimer()
        {
            attackTimer = Random.Range(0.1f, maxAttackTimer);
        }

        void SetBlockTimer()
        {
            blockTimer = Random.Range(0.1f, maxBlockTimer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Initialize()
        {
            ResetTimers();
            Engine.Log.Print("AIDeathMatch initialized.");
        }

        Transform GetNearest(out CharacterMovement enemyMovement)
        {
            enemyMovement = null;
            var characters = Character.allCharacters;
            float dis = Mathf.Infinity;
            Transform newTarget = null;
            float dis2;
            for (int i = 0; i < characters.Count; i++)
            {
                dis2 = Vector3.Distance(characterTransform.position, position); 
                if(dis2 < dis && characters[i] != characterMovementAI.character)
                {
                    newTarget = characters[i].transform;
                    enemyMovement = characters[i].movement;
                    dis2 = dis;
                }
            }
            return newTarget;
        }

        void HandlePath()
        {
            if(Vector3.Distance(targetPosition, destination) > 1)
            {
                timer = 0;
                destination = target.position;
                AIMovement.path = AIMovement.pathMovement.GetPath(targetPosition);
            }
        }

        void HandleAttack()
        {
            AIMovement.Attack();
        }

        //void ShieldUp()
        //{
        //    if (!AIMovement.shieldUp && AIMovement.Shield)
        //    {
        //        AIMovement.shieldUp = true;
        //        AIMovement.Shield.Use();
        //        AIMovement.StartCoroutine(BlockingTime(Random.Range(1,maxBlockingTime)));
        //        if (Engine.Math.Probability(0.5f))
        //        {
        //            //AIMovement.Roll(true);
        //            AIMovement.StartCoroutine(IsRolling());
        //        }

        //    }
        //}

        //void ShieldDown()
        //{
        //    if (AIMovement.shieldUp && AIMovement.Shield)
        //    {
        //        AIMovement.Shield.StopUsing();
        //    }
        //}

        IEnumerator BlockingTime(float blockTime)
        {
            while(blockTime > 0)
            {
                blockTime -= deltaTime;
                if (target && DistanceToTarget() > 2)
                    blockTime = 0;
                yield return null;
            }
            ResetTimers(true);
            //ShieldDown();
        }

        IEnumerator IsRolling()
        {
            yield return new WaitForSeconds(0.5f);
            while (AIMovement.isRolling)
            {
                //Debug.Log("IS ROLLING");
                AIMovement.ForwardPower = 1;
                AIMovement.Move();
                yield return null;
            }
        }

        void ResetTarget()
        {
            target = null;
            enemyCharacter = null;
        }
    }
}