using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IBrickColorsService>().To<BrickColorsService>().AsSingle();
        Container.Bind<ScoreService>().To<ScoreService>().AsSingle();

        //Container.BindInterfacesAndSelfTo<BrickColorsService>().AsSingle();
    }

}
