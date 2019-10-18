public static class WeaponList
{
    public static Weapon basic_weapon = new Weapon
    {
        frequency = 30,
        durability = 500,
        projectileSpeed = 0.03f,
        lastShot = 0
    };
    
    public static Weapon paint_weapon = new Weapon
    {
        frequency = 1,
        durability = 500,
        projectileSpeed = 0.05f,
        lastShot = 0
    };
}