using UnityEngine;

[System.Serializable]
public class BaseModel
{

}

public class BaseController<M> where M : BaseModel
{
    protected M Model;

    public virtual void Setup(M model)
    {
        Model = model;
    }

}

public abstract class BaseView<M, C> : MonoBehaviour
    where M : BaseModel, new()
    where C : BaseController<M>, new()
{
    protected M Model;
    protected C Controller;

    public virtual void Awake()
    {
        Model = new M();
        Controller = new C();
        Controller.Setup(Model);
    }

    public abstract void UpdateViewFromModel();
}
