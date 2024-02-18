using Content.Server.Chat.Systems;
using Content.Server.Radio.Components;
using Content.Shared.Verbs;
namespace Content.Server.Radio.EntitySystems;

public sealed class frequencychanger : EntitySystem
{
    [Dependency] public readonly ChatSystem _chat = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FrequencyComponent, GetVerbsEvent<ActivationVerb>>(AddFreq);

    }
    private void AddFreq(EntityUid uid, FrequencyComponent component, GetVerbsEvent<ActivationVerb> args)
    {

        ActivationVerb verb = new()
        {
            Text = "прибавить к частоте 1",
            Act = () => ChangeFrequency(uid,1)
        };
        args.Verbs.Add(verb);
        ActivationVerb verb1 = new()
        {
            Text = "прибавить к частоте 10",
            Act = () => ChangeFrequency(uid,10)
        };
        args.Verbs.Add(verb1);
        ActivationVerb verb2 = new()
        {
            Text = "уменьшить частоту на 1",
            Act = () => ChangeFrequency(uid,-1)
        };
        args.Verbs.Add(verb2);
        ActivationVerb verb3 = new()
        {
            Text = "уменьшить частоту на 10",
            Act = () => ChangeFrequency(uid,-10)
        };
        args.Verbs.Add(verb3);
    }

    private void ChangeFrequency(EntityUid uid,int freq)
    {
        if (TryComp(uid, out FrequencyComponent? radio))
        {
            if (radio.frequency + freq < 100)
            {
                _chat.TrySendInGameICMessage(uid, "слишком низкая частота", InGameICChatType.Whisper, ChatTransmitRange.GhostRangeLimit, nameOverride: "Рация", checkRadioPrefix: false);
                return;
            }
            if (radio.frequency + freq > 999)
            {
                _chat.TrySendInGameICMessage(uid, "слишком высокая частота", InGameICChatType.Whisper, ChatTransmitRange.GhostRangeLimit, nameOverride: "Рация", checkRadioPrefix: false);
                return;
            }

            radio.frequency += freq;
            _chat.TrySendInGameICMessage(uid, "частота "+radio.frequency.ToString(), InGameICChatType.Whisper, ChatTransmitRange.GhostRangeLimit, nameOverride: "Рация", checkRadioPrefix: false);
        }
    }
}
