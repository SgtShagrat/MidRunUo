using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public interface IXmlAttachment
    {
        ASerial Serial { get; }

        string Name { get; set; }

        TimeSpan Expiration { get; set; }

        DateTime ExpirationEnd { get; }

        DateTime CreationTime { get; }

        bool Deleted { get; }

        bool DoDelete { get; set; }

        bool CanActivateInBackpack { get; }

        bool CanActivateEquipped { get; }

        bool CanActivateInWorld { get; }

        bool HandlesOnSpeech { get; }

        void OnSpeech( SpeechEventArgs args );

        bool HandlesOnMovement { get; }

        void OnMovement( MovementEventArgs args );

        bool HandlesOnKill { get; }

        void OnKill( Mobile killed, Mobile killer );

        void OnBeforeKill( Mobile killed, Mobile killer );

        bool HandlesOnKilled { get; }

        void OnKilled( Mobile killed, Mobile killer );

        void OnBeforeKilled( Mobile killed, Mobile killer );

        /*
		bool HandlesOnSkillUse { get; }

		void OnSkillUse( Mobile m, Skill skill, bool success);
		*/

        object AttachedTo { get; set; }

        object OwnedBy { get; set; }

        bool CanEquip( Mobile from );

        void OnEquip( Mobile from );

        void OnRemoved( object parent );

        void OnAttach();

        void OnReattach();

        void OnUse( Mobile from );

        void OnUser( object target );

        bool BlockDefaultOnUse( Mobile from, object target );

        bool OnDragLift( Mobile from, Item item );

        string OnIdentify( Mobile from );

        string DisplayedProperties( Mobile from );

        void AddProperties( ObjectPropertyList list );

        string AttachedBy { get; }

        void OnDelete();

        void Delete();

        void InvalidateParentProperties();

        void SetAttachedBy( string name );

        void OnTrigger( object activator, Mobile from );

        void OnWeaponHit( Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven );

        int OnArmorHit( Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven );

        void Serialize( GenericWriter writer );

        void Deserialize( GenericReader reader );

    }
}