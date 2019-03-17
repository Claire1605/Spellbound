void setup()
{
 size(2048, 2048); 
 
 background(0);
 noStroke();
 
 for (int star = 0; star < 260; star++)
 {
   fill(random(255));
 
   float radius = random(6);
   ellipse(random(width - radius) + radius, random(height - radius) + radius, radius, radius);
 }
 
 save("stars.png");
}
