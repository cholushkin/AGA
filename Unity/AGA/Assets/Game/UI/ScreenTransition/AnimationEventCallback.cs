using GameLib.UI;
using UnityEngine;

public class AnimationEventCallback : MonoBehaviour
{
    public ScreenTransition ScreenTransitionEffect;

    public void OnAnimationFinished()
    {
        ScreenTransitionEffect.OnAnimationFinishCallBack(gameObject.name);
    }
}
