import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import type { BossPullSeries } from '../types';

const LINE_COLORS = ['#3fc7c1', '#d9a441', '#e8683f', '#8b93a1'];

function toChartData(series: BossPullSeries[]) {
  const maxPulls = Math.max(...series.map((s) => s.points.length));
  const rows: Record<string, number | string>[] = [];
  for (let i = 0; i < maxPulls; i++) {
    const row: Record<string, number | string> = { pull: i + 1 };
    for (const s of series) {
      if (s.points[i]) {
        row[s.guild.name] = i + 1;
      }
    }
    rows.push(row);
  }
  return rows;
}

export function PullChart({ series }: { series: BossPullSeries[] }) {
  const data = toChartData(series);

  return (
    <div className="rounded-[10px] border border-border bg-card p-4">
      <h3 className="eyebrow mb-4 text-xs text-text-muted">Pull-Verlauf im Vergleich</h3>
      <ResponsiveContainer width="100%" height={320}>
        <LineChart data={data}>
          <CartesianGrid stroke="#232a34" strokeDasharray="3 3" />
          <XAxis
            dataKey="pull"
            stroke="#8b93a1"
            tick={{ fontFamily: 'JetBrains Mono', fontSize: 11 }}
            label={{ value: 'Pull #', position: 'insideBottom', offset: -5, fill: '#8b93a1' }}
          />
          <YAxis stroke="#8b93a1" tick={{ fontFamily: 'JetBrains Mono', fontSize: 11 }} />
          <Tooltip
            contentStyle={{
              background: '#12161c',
              border: '1px solid #232a34',
              borderRadius: 8,
              fontFamily: 'Inter',
              fontSize: 12,
            }}
          />
          {series.map((s, i) => (
            <Line
              key={s.guild.id}
              type="monotone"
              dataKey={s.guild.name}
              stroke={LINE_COLORS[i % LINE_COLORS.length]}
              strokeWidth={2}
              dot={false}
            />
          ))}
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}
