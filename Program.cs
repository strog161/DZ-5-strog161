using System;
using System.Collections.Generic;

internal class Program
{
    public enum AttackType
    {
        Damage,
        Self,
        Heal
    }

    static void Main(string[] args)
    {
        string? name; // имя игрока
        float maxPlayerHealth = 1000f; // хп игрока
        float maxEnemyHealth = 2000f; // хп врага

        float currentPlayerHealth = maxPlayerHealth; // текущее хп игрока
        float currentEnemyHealth = maxEnemyHealth; // текущее хп врага

        float proc = 0.1f;

        float playerDamage = 50f;
        float enemyDamage = 55f;
        float firebollDamage = 200f;
        float enemyHealDamage = 100f;
        float playerHealDamage = 100f;

        string? input;

        bool enemyDamageWithWeapon = false;//оружие врага
        int commandCount = 3; // колличество действий противника
        int enemySecondAbilityModifier = 3; // модификатор урона противника

        Random randomizer = new Random();

        Dictionary<AttackType, List<float>> damageToPlayer = new();
        Dictionary<AttackType, List<float>> damageToEnemy = new();

        damageToPlayer[AttackType.Damage] = new List<float>();
        damageToPlayer[AttackType.Self] = new List<float>();
        damageToPlayer[AttackType.Heal] = new List<float>();

        damageToEnemy[AttackType.Damage] = new List<float>();
        damageToEnemy[AttackType.Self] = new List<float>();
        damageToEnemy[AttackType.Heal] = new List<float>();      

        Console.WriteLine("Введите ваше имя: ");
        name = Console.ReadLine(); // считывание имени игрока

        Console.Clear();
        Console.WriteLine($"Добро пожаловать в игру, {name}\n");
        
        while (currentEnemyHealth > 0 && currentPlayerHealth > 0) // основной боевой цикл
        {
            HealthAndActionMassege();

            input = Console.ReadLine(); // считывание номера действия
            bool playShield = false; // щит
            bool playerDamageWithWeapon = false; // оружие           
            switch (input) // варианты действий игрока
            {
                case "1": // урон оружием по врагу
                    playerDamageWithWeapon = true;
                    currentEnemyHealth -= playerDamage;
                    damageToEnemy[AttackType.Damage].Add(playerDamage);
                    break;
                case "2":
                    playShield = true;
                    break;
                case "3": // урон огненым шаром по врагу
                    currentEnemyHealth -= firebollDamage;
                    damageToEnemy[AttackType.Damage].Add(firebollDamage);
                    break;
                case "4"://исцеление
                    if (!enemyDamageWithWeapon)
                    {
                        currentPlayerHealth += playerHealDamage;
                        damageToEnemy[AttackType.Heal].Add(playerHealDamage);
                        if (currentPlayerHealth > maxPlayerHealth)
                        {
                            currentPlayerHealth = maxPlayerHealth;
                        }
                    }
                    break;
                default:
                    Error();
                    break;
            }
            enemyDamageWithWeapon = false;
            int enemyCommand = randomizer.Next(1, commandCount + 1);            
            switch (enemyCommand) // варианты действий противника
            {
                case 1: // простой удар от врага если нет щита
                    if (!playShield)
                    {
                        currentPlayerHealth -= enemyDamage;
                        damageToPlayer[AttackType.Damage].Add(enemyDamage);
                        enemyDamageWithWeapon = true;
                    }
                    break;
                case 2:
                    if (!playShield) // если щит не активен, то бьет себя и бьет игрока в 3 раза сильнее
                    {
                        currentEnemyHealth -= enemyDamage;
                        currentPlayerHealth -= enemyDamage * enemySecondAbilityModifier;
                        damageToPlayer[AttackType.Self].Add(enemyDamage);
                        damageToPlayer[AttackType.Damage].Add(enemyDamage * enemySecondAbilityModifier);
                    }
                    break;
                case 3: // если врага ударить оружием то он наносит себе урон, если нет то лечится на 100
                    if (playerDamageWithWeapon)
                    {
                        currentEnemyHealth -= enemyHealDamage;
                        damageToPlayer[AttackType.Self].Add(enemyHealDamage);
                    }
                    else
                    {
                        currentEnemyHealth += enemyHealDamage;
                        damageToPlayer[AttackType.Heal].Add(enemyHealDamage);
                        if (currentEnemyHealth > maxEnemyHealth)
                        {
                            currentEnemyHealth = maxEnemyHealth;
                        }
                    }
                    break;
                default:
                    Error();
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            WriteDamagesFromTo(damageToEnemy, name, "Enemy");
            WriteDamagesFromTo(damageToPlayer, "Enemy", name);
            Console.ResetColor();

            EndTurn();
        }
        void HealthAndActionMassege()//вывод здоровья в начале раунда и выбор действия
        {          
            if (currentPlayerHealth > maxPlayerHealth * proc)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ваше здоровье: {currentPlayerHealth}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ваше здоровье: {currentPlayerHealth}");

                Console.ResetColor();
            }
            if (currentEnemyHealth > maxEnemyHealth * proc)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Здоровье противника: {currentEnemyHealth}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Здоровье противника: {currentEnemyHealth}");
                Console.ResetColor();
            }
            Console.WriteLine($"{name}! Выберите действие:\n" +
                $"1. Ударить оружием (урон {playerDamage})\n" +
                $"2. Щит: следующая атака противника не наносит урон\n" +
                $"3. Огненый шар: наносит урон в размере {firebollDamage}\n" +
                $"4. Исцелиться на 100 ед.\n");
        }
    }

    static void EndTurn() // метод конец раунда
    {
        Console.WriteLine("Для продолжения нажмите любую клавишу.");
        Console.ReadKey();
        Console.Clear();
    }

    static void DamageMassage(string damager, string defender, float damage) // метод вывода урона
    {
        Console.WriteLine($"Игрок {damager} наносит игроку {defender} урон в размере {damage} единиц.");
    }

    static void SelfDamageMassage(string damager, float damage)
    {
        Console.WriteLine($"Игрок {damager} нанес {damage} урона сам себе");
    }

    static void HealMassage(string healer, float healValue) // метод вывода лечения
    {
        Console.WriteLine($"Игрок {healer} восстановил себе здоровье в размере {healValue} единиц");
    }

    static void WriteDamageWithType(AttackType attackType, string damagerName, string defenderName, List<float> damages) // метод определения действия
    {
        foreach (var damage in damages)
        {
            switch (attackType)
            {
                case AttackType.Damage:
                    DamageMassage(damagerName, defenderName, damage);
                    break;
                case AttackType.Self:
                    SelfDamageMassage(damagerName, damage);
                    break;
                case AttackType.Heal:
                    HealMassage(damagerName, damage);
                    break;
                default:
                    break;
            }
        }
        damages.Clear();
    }

    static void WriteDamagesFromTo(Dictionary<AttackType, List<float>> damages, string damagerName, string defenderName) // метод нанесения урона
    {
        foreach (var damage in damages)
        {
            WriteDamageWithType(damage.Key, damagerName, defenderName, damage.Value);
        }
    }
    static void Error()
    {
        Console.WriteLine("Команда не распознана");
    }
    
}
