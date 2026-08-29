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

const MS_PER_HOUR = 3_600_000;

/**
 * Eine Zeile je Pull-Nummer, pro Gilde die bis dahin verstrichene Zeit seit ihrem
 * eigenen ersten Pull. Damit ist die flachere Linie die schnellere Gilde. Gilden
 * mit weniger Pulls lassen ihren Wert ab dort weg, ihre Linie endet dann einfach.
 */
function toChartData(series: BossPullSeries[]) {
  if (series.length === 0) return [];

  const maxPulls = Math.max(...series.map((s) => s.points.length));
  const rows: Record<string, number | undefined>[] = [];

  for (let i = 0; i < maxPulls; i++) {
    const row: Record<string, number | undefined> = { pull: i + 1 };
    for (const s of series) {
      const first = s.points[0];
      const point = s.points[i];
      if (!first || !point) continue;

      const hours =
        (new Date(point.timestamp).getTime() - new Date(first.timestamp).getTime()) / MS_PER_HOUR;
      if (!Number.isFinite(hours)) continue;

      row[s.guild.name] = Math.round(hours * 10) / 10;
    }
    rows.push(row);
  }
  return rows;
}

export function PullChart({ series }: { series: BossPullSeries[] }) {
  const data = toChartData(series);

  if (data.length === 0) {
    return (
      <div className="rounded-[10px] border border-border bg-card p-4 text-sm text-text-muted">
        Für diesen Boss liegen noch keine Pull-Daten vor.
      </div>
    );
  }

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
          <YAxis
            stroke="#8b93a1"
            tick={{ fontFamily: 'JetBrains Mono', fontSize: 11 }}
            label={{
              value: 'Stunden seit Pull 1',
              angle: -90,
              position: 'insideLeft',
              fill: '#8b93a1',
            }}
          />
          <Tooltip
            formatter={(value, name) => [`${value} h`, name]}
            labelFormatter={(label) => `Pull #${label}`}
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
