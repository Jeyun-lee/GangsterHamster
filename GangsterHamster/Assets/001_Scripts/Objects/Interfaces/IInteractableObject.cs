using System;
using UnityEngine;


namespace Objects.Interactable
{
    /// <summary>
    /// 상호 작용 가능한 오브젝트가 구현해야 함
    /// </summary>
    [Obsolete]
    public interface IInteractableObject
    {
        /// <summary>
        /// 상호 작용 용도로 호출되어야 함
        /// </summary>
        /// <param name="callback">callback (if needed)</param>
        public void Interact(Action callback = null);

        /// <summary>
        /// 사용할 때 같이 호출해줘야 하는 함수
        /// </summary>
        /// <param name="callback">callback (if needed)</param>
        public void Initialize(Action callback = null);

        /// <summary>
        /// 사용하고 난 후 호출해야함
        /// </summary>
        public void Release();

        /// <summary>
        /// 충돌 용도로 호출되어야 함
        /// </summary>
        /// <param name="collision">본인의 GameObject</param>
        /// <param name="callback">callback (if needed)</param>
        public void Collision(UnityEngine.GameObject collision, Action callback = null); // 여기 두기 애매하긴 함
    }
}