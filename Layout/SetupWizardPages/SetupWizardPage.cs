using System.IO.Pipelines;
using Kt.Data;
using Microsoft.AspNetCore.Components;

namespace Kt.Layout;

public abstract class SetupWizardPage: LayoutComponentBase
{

    [CascadingParameter] protected SetupWizard? Wizard {get; set;}
    [Inject] protected TeamsDatabase? teamdb {get; set;}
    [Inject] protected RuleDatabase? ruledb {get; set;}

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Wizard is null)
            return;
            
        Wizard.AddPage(this);
    }

    protected bool IsActivePage
    {
        get
        {
            return Wizard is not null && ReferenceEquals(this, Wizard.ActivePage);
        }
    }

    public virtual bool Provides(string field)
    {
        return false;
    }

    public virtual object? GetValue(string field)
    {
        return null;
    }

    public abstract void Reset();
    public virtual void OnShow() {}
    public abstract bool CanAdvance();
}