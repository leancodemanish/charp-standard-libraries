namespace LeanCode.Thor.Standard.Pipes
{
    public delegate void ConnectionMessageEventHandler<TRead, TWrite>(TWrite message);
        
}