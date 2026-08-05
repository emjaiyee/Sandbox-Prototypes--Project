// Interface for modular interaction system
using UnityEngine;

public interface IInteractable
{
    string InteractionPrompt { get; }
    void Interact(Transform interactorHand);
}
