using RootMobile.Views.GameContecst;

public interface IStatusEffect
{
    string Name { get; }
    int Duration { get; set; }

    void Apply(Character target);
    void Tick(Character target);
}