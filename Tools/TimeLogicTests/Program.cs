using System;
using System.Collections.Generic;
int checks=0;
void Check(bool ok,string label) {checks++;if(!ok)throw new Exception(label);}
void Reject(Action action,string label) {try {action();}catch(ArgumentOutOfRangeException){Check(true,label);return;}Check(false,label);}
var clock=new GameClock();var listener=new Listener();clock.Subscribe(listener);clock.Subscribe(listener);
Check(clock.Day==1 && clock.Hour==7 && clock.Minute==0,"start at 07:00");
clock.Advance(12.5);
Check(clock.Hour==8 && clock.Minute==0,"12.5 seconds per game hour");
Check(listener.Elapsed==3600 && listener.Hours.Count==1,"subscribe is idempotent");
clock.Advance(200);
Check(clock.Day==2 && clock.Hour==0 && clock.Minute==0,"midnight from 07:00");
Check(listener.Days.Count==1 && listener.Days[0]==2,"ordered day notification");
Check(listener.Hours[^1]==(2,0),"midnight hour follows day");
clock.Advance(600);
Check(clock.Day==4 && clock.Hour==0 && listener.Days.Count==3,"large tick emits every day");
double before=listener.Elapsed;clock.Unsubscribe(listener);clock.Advance(1);
Check(listener.Elapsed==before,"unsubscribe");
Reject(()=>clock.Advance(-1),"negative time rejected");Reject(()=>clock.Advance(double.NaN),"NaN rejected");Reject(()=>clock.Advance(double.PositiveInfinity),"infinite time rejected");
Check(GameClock.FormatDuration(600)=="10 phút","tilling duration");
Check(GameClock.FormatDuration(86400)=="1 ngày","day duration");
Check(GameClock.FormatDuration(90060)=="1 ngày 1 giờ 1 phút","duration without decimal days");
var definitions=CropGameRules.CreateDefinitions(86400);
int[] days={1,2,2,2,4,5,2,3},yields={3,3,3,3,4,4,2,2};
for(int i=0;i<8;i++)
{
 Check(definitions[i].Stage1Duration==days[i]*86400 && definitions[i].Stage2Duration==days[i]*86400,"agreed crop duration "+i);
 Check(definitions[i].BaseYield==yields[i],"agreed yield "+i);
 Check(definitions[i].DroughtPenaltyThreshold==43200 && definitions[i].DroughtDeathThreshold==86400,"water thresholds "+i);
}
Check(definitions[3].RegrowthDuration==86400 && definitions[4].RegrowthDuration==172800 && definitions[5].RegrowthDuration==172800,"regrowth intervals");
var rice=new SingleHarvestCrop(definitions[0]);var cropClock=new GameClock();cropClock.Subscribe(new CropListener(rice));
rice.Water();cropClock.Advance(300);
Check(rice.State==CropState.Stage2 && rice.WateringCount==0,"one day completes first rice stage");
rice.Water();cropClock.Advance(300);
Check(rice.State==CropState.Ripe,"two watered days produce ripe rice");
cropClock.Advance(299);
Check(rice.State==CropState.Ripe,"ripe crop survives before rot threshold");
cropClock.Advance(1);
Check(rice.State==CropState.Dead,"ripe crop rots after one game day");
var thirsty=new SingleHarvestCrop(definitions[0]);var dryClock=new GameClock();dryClock.Subscribe(new CropListener(thirsty));dryClock.Advance(150);
Check(thirsty.HasDroughtPenalty && thirsty.State==CropState.Stage1,"12-hour penalty");
dryClock.Advance(150);Check(thirsty.State==CropState.Dead,"24-hour drought death");
var till=new FarmPlot(600);var tillClock=new GameClock();tillClock.Subscribe(new PlotListener(till));till.StartTilling();tillClock.Advance(600.0/288);
Check(till.Status==FarmPlotStatus.ReadyToSow,"10-minute tilling with shared clock");
var farm=new SecondSpring.Prototype.Farm();farm.Add(0,SecondSpring.Prototype.Species.Chicken);farm.FeedPen(0);farm.Advance(212.5);
Check(farm.Day==2 && farm.Minute==0 && farm.Pens[0].Animals[0].Growth==1,"Vinh prototype midnight growth preserved");
farm.Advance(300);Check(farm.Day==3 && farm.Pens[0].Animals[0].Growth==1,"missed feed does not grow");
var both=new GameClock();var linkedFarm=new SecondSpring.Prototype.Farm(both);var linkedCrop=new SingleHarvestCrop(definitions[0]);both.Subscribe(new CropListener(linkedCrop));linkedCrop.Water();linkedFarm.Add(0,SecondSpring.Prototype.Species.Chicken);linkedFarm.FeedPen(0);both.Advance(300);
Check(linkedFarm.Day==2 && linkedFarm.Pens[0].Animals[0].Growth==1 && linkedCrop.State==CropState.Stage2,"plants and Vinh prototype can share one clock instance");
Console.WriteLine($"Passed {checks} clock and growth checks.");
sealed class Listener:IGameTimeListener { public double Elapsed;public List<int> Days=new();public List<(int,int)> Hours=new();public void OnTimeElapsed(float dt){Elapsed+=dt;}public void OnDayChanged(int d){Days.Add(d);}public void OnHourChanged(int d,int h){Hours.Add((d,h));} }
sealed class CropListener:IGameTimeListener { private readonly Crop _crop;public CropListener(Crop crop){_crop=crop;}public void OnTimeElapsed(float dt){_crop.UpdateGrowth(dt);}public void OnDayChanged(int d){}public void OnHourChanged(int d,int h){} }
sealed class PlotListener:IGameTimeListener { private readonly FarmPlot _plot;public PlotListener(FarmPlot plot){_plot=plot;}public void OnTimeElapsed(float dt){_plot.UpdateTilling(dt);}public void OnDayChanged(int d){}public void OnHourChanged(int d,int h){} }
